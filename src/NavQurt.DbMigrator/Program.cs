using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NavQurt.Core.Entities;
using NavQurt.DbMigrator;
using NavQurt.Infrastructure;
using NavQurt.Infrastructure.Data;
using NavQurt.Server.Extensions;
using NavQurt.Server.HostedServices;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting NavQurt DbMigrator");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddApplicationOptions(ctx.Configuration);
        services.AddInfrastructureLayer(
            ctx.Configuration,
            optionsAction: options =>
            {
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
        services.AddHostedService<IdentitySeederService>();
        services.AddHostedService<OpenIddictSeederService>();
    })
    .UseSerilog()
    .Build();

await host.RunAsync();
