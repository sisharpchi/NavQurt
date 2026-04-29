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

}
