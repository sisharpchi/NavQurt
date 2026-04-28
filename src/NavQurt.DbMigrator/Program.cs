using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NavQurt.Core.Entities;
using NavQurt.DbMigrator;
using NavQurt.Infrastructure.Data;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting NavQurt DbMigrator");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        var connectionString = ctx.Configuration.GetSection("ConnectionStrings")["MainDatabase"]
            ?? "Server=127.0.0.1;port=5432;Database=navqurt_db;Uid=postgres;Pwd=1";
        services.AddDbContext<MainDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: ["08P01", "08006", "08001", "08004"]);
            });
            options.UseOpenIddict<OpenIdApplication, OpenIdAuthorization, OpenIdScope, OpenIdToken, long>();
        });

        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<MainDbContext>()
                    .ReplaceDefaultEntities<OpenIdApplication, OpenIdAuthorization, OpenIdScope, OpenIdToken, long>();
            });

        services.AddHostedService<MainDatabaseMigratorService>();
    })
    .UseSerilog()
    .Build();

await host.RunAsync();
