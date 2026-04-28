using NavQurt.Core.Entities;
using NavQurt.Infrastructure.Data;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using NavQurt.Server.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace NavQurt.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingletonOption<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where TOptions : class, new()
    {
        var options = configuration.GetSection(sectionName).Get<TOptions>() ?? new TOptions();
        services.AddSingleton(options);
        return services;
    }

    public static IServiceCollection AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingletonOption<AdminSeedOptions>(configuration, AdminSeedOptions.Key);
        services.AddSingletonOption<OpenIddictSeedOptions>(configuration, OpenIddictSeedOptions.Key);
        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "NavQurt API",
                Version = "v1"
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "NavQurt API v1");
            options.DocumentTitle = "NavQurt API Documentation";
            options.DocExpansion(DocExpansion.List);
            options.RoutePrefix = "swagger";
        });

        return app;
    }

    public static IServiceCollection AddOpenTelemetryEx(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                serviceName: "NavQurt.Server",
                serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown"))
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddHttpClientInstrumentation();
                metrics.AddRuntimeInstrumentation();
                metrics.AddProcessInstrumentation();
                metrics.AddPrometheusExporter(options =>
                {
                    options.DisableTotalNameSuffixForCounters = true;
                });
            });

        return services;
    }

    public static IServiceCollection RegisterOpenIddict(this IServiceCollection services)
    {
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<MainDbContext>()
                    .ReplaceDefaultEntities<OpenIdApplication, OpenIdAuthorization, OpenIdScope, OpenIdToken, long>();
            })
            .AddServer(options =>
            {
                options.RegisterScopes("read", "write", OpenIddictConstants.Scopes.OfflineAccess);

                options.SetTokenEndpointUris("/security/oauth/token")
                    .SetRevocationEndpointUris("/security/oauth/revoke");

                options.AllowClientCredentialsFlow()
                    .AllowPasswordFlow()
                    .AllowRefreshTokenFlow();

                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                options.UseAspNetCore()
                    .DisableTransportSecurityRequirement()
                    .EnableTokenEndpointPassthrough();

                options.SetRefreshTokenLifetime(TimeSpan.FromDays(7));
                options.SetAccessTokenLifetime(TimeSpan.FromHours(24));
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        return services;
    }
}
