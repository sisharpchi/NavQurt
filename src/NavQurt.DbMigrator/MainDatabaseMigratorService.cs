using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NavQurt.Infrastructure.Data;

namespace NavQurt.DbMigrator;

public class MainDatabaseMigratorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MainDatabaseMigratorService> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public MainDatabaseMigratorService(
        IServiceScopeFactory scopeFactory,
        ILogger<MainDatabaseMigratorService> logger,
        IHostApplicationLifetime applicationLifetime)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Applying MainDbContext migrations...");
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();
            await MarkInitialMigrationAppliedForExistingDatabaseAsync(dbContext, stoppingToken);
            await dbContext.Database.MigrateAsync(stoppingToken);
            _logger.LogInformation("MainDbContext migrations applied.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MainDbContext migration failed.");
            Environment.ExitCode = 1;
        }
        finally
        {
            _applicationLifetime.StopApplication();
        }
    }

    private async Task MarkInitialMigrationAppliedForExistingDatabaseAsync(MainDbContext dbContext, CancellationToken cancellationToken)
    {
        const string initialMigration = "20260428072028_InitialIdentityOpenIddict";

        var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync(cancellationToken);
        if (appliedMigrations.Contains(initialMigration))
        {
            return;
        }

        var openIdTableExists = await dbContext.Database
            .SqlQueryRaw<bool>("SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'OpenIddictEntityFrameworkCoreApplications') AS \"Value\"")
            .SingleAsync(cancellationToken);

        if (!openIdTableExists)
        {
            return;
        }

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            VALUES ('20260428072028_InitialIdentityOpenIddict', '8.0.26')
            ON CONFLICT ("MigrationId") DO NOTHING;
            """,
            cancellationToken);

        _logger.LogInformation("Marked existing identity/openiddict schema as migrated.");
    }
}
