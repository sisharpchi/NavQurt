using Microsoft.AspNetCore.Identity;
using NavQurt.Core.Entities;
using NavQurt.Core.Enumerations;
using NavQurt.Server.Options;
using NavQurt.Shared.Claims;

namespace NavQurt.Server.HostedServices;

public class IdentitySeederService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AdminSeedOptions _adminSeedOptions;
    private readonly ILogger<IdentitySeederService> _logger;

    public IdentitySeederService(
        IServiceProvider serviceProvider,
        AdminSeedOptions adminSeedOptions,
        ILogger<IdentitySeederService> logger)
    {
        _serviceProvider = serviceProvider;
        _adminSeedOptions = adminSeedOptions;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        await EnsureRoleAsync(roleManager, AppRoles.SuperAdmin);
        await EnsureSuperAdminClaimsAsync(roleManager);
        await EnsureConfiguredAdminAsync(userManager, stoppingToken);
    }

    private static async Task EnsureRoleAsync(RoleManager<AppRole> roleManager, string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        await roleManager.CreateAsync(new AppRole(roleName)
        {
            DisplayName = "Super Admin"
        });
    }

    private static async Task EnsureSuperAdminClaimsAsync(RoleManager<AppRole> roleManager)
    {
        var role = await roleManager.FindByNameAsync(AppRoles.SuperAdmin);
        if (role == null)
        {
            return;
        }

        var existingClaims = await roleManager.GetClaimsAsync(role);
        foreach (var claim in ClaimsStore.AllClaims.Select(x => x.Claim))
        {
            if (existingClaims.Any(x => x.Type == claim.Type && x.Value == claim.Value))
            {
                continue;
            }

            await roleManager.AddClaimAsync(role, claim);
        }
    }

    private async Task EnsureConfiguredAdminAsync(UserManager<User> userManager, CancellationToken ct)
    {
        var userName = _adminSeedOptions.UserName;
        var password = _adminSeedOptions.Password;

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
            user = new User
            {
                UserName = userName,
                PhoneNumber = _adminSeedOptions.PhoneNumber,
                FirstName = _adminSeedOptions.FirstName,
                LastName = _adminSeedOptions.LastName
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                _logger.LogWarning("Admin user seed failed: {Errors}", string.Join("; ", createResult.Errors.Select(x => x.Description)));
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(user, AppRoles.SuperAdmin))
        {
            await userManager.AddToRoleAsync(user, AppRoles.SuperAdmin);
        }

        _logger.LogInformation("Admin user seed checked for {UserName}.", userName);
    }
}
