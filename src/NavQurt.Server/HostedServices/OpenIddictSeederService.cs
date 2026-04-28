using OpenIddict.Abstractions;
using NavQurt.Server.Options;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace NavQurt.Server.HostedServices;

public class OpenIddictSeederService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly OpenIddictSeedOptions _seedOptions;
    private readonly ILogger<OpenIddictSeederService> _logger;

    public OpenIddictSeederService(
        IServiceProvider serviceProvider,
        OpenIddictSeedOptions seedOptions,
        ILogger<OpenIddictSeederService> logger)
    {
        _serviceProvider = serviceProvider;
        _seedOptions = seedOptions;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        await SeedWebClientAsync(applicationManager, stoppingToken);
        await SeedServiceClientAsync(applicationManager, stoppingToken);

        _logger.LogInformation("OpenIddict clients seeded successfully.");
    }

    private async Task SeedWebClientAsync(IOpenIddictApplicationManager applicationManager, CancellationToken ct)
    {
        var clientId = _seedOptions.WebClientId;
        if (await applicationManager.FindByClientIdAsync(clientId, ct) is not null)
        {
            return;
        }

        var permissions = new[]
        {
            Permissions.Endpoints.Token,
            Permissions.GrantTypes.Password,
            Permissions.GrantTypes.RefreshToken,
            Permissions.Scopes.Profile,
            Permissions.Scopes.Email,
            Permissions.Scopes.Roles,
            Permissions.Prefixes.Scope + "read",
            Permissions.Prefixes.Scope + "write",
            Permissions.Prefixes.Scope + Scopes.OfflineAccess
        };

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            DisplayName = _seedOptions.WebClientDisplayName
        };
        foreach (var permission in permissions)
        {
            descriptor.Permissions.Add(permission);
        }

        await applicationManager.CreateAsync(descriptor, ct);
        _logger.LogInformation("Created OpenIddict public client: {ClientId}", clientId);
    }

    private async Task SeedServiceClientAsync(IOpenIddictApplicationManager applicationManager, CancellationToken ct)
    {
        var clientId = _seedOptions.ServiceClientId;
        if (await applicationManager.FindByClientIdAsync(clientId, ct) is not null)
        {
            return;
        }

        var permissions = new[]
        {
            Permissions.Endpoints.Token,
            Permissions.GrantTypes.ClientCredentials,
            Permissions.Prefixes.Scope + "read",
            Permissions.Prefixes.Scope + "write"
        };

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            DisplayName = _seedOptions.ServiceClientDisplayName
        };
        foreach (var permission in permissions)
        {
            descriptor.Permissions.Add(permission);
        }

        await applicationManager.CreateAsync(descriptor, _seedOptions.ServiceClientSecret, ct);
        _logger.LogInformation("Created OpenIddict confidential client: {ClientId}", clientId);
    }
}
