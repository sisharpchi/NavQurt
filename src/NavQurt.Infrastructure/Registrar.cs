using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NavQurt.Core.Entities;
using NavQurt.Core.Persistence;
using NavQurt.Infrastructure.Data;
using NavQurt.Infrastructure.Helpers;
using NavQurt.Infrastructure.Options;
using NavQurt.Infrastructure.Persistence;

namespace NavQurt.Infrastructure;

public static class Registrar
{
    private static void AddIdentity(IServiceCollection services, Action<IdentityBuilder>? configureIdentity = null)
    {
        var identityBuilder = services
            .AddDefaultIdentity<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.User.AllowedUserNameCharacters =
                    "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789_.@+";
                options.User.RequireUniqueEmail = false;
            })
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<MainDbContext>();

        configureIdentity?.Invoke(identityBuilder);
    }

    private static void AddMainDatabase(
        IServiceCollection services,
        IConfiguration configuration,
        string connectionSection,
        Action<DbContextOptionsBuilder>? optionsAction = null)
    {
        var connectionString = configuration.GetConnectionString(connectionSection)
            ?? ConnectionStrings.LocalMain;

        services.AddSingleton(new MainDbContextOptions
        {
            ConnectionString = connectionString
        });

        services.AddDbContext<MainDbContext>(options =>
        {
            DbContextHelpers.ConfigurePostgreSql(connectionString, options);
            optionsAction?.Invoke(options);
        });
    }

    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IdentityBuilder>? configureIdentity = null,
        Action<DbContextOptionsBuilder>? optionsAction = null,
        string mainDbConnectionSection = "MainDatabase")
    {
        AddIdentity(services, configureIdentity);
        AddMainDatabase(services, configuration, mainDbConnectionSection, optionsAction);

        services.AddScoped<IMainRepository, MainRepository>();
        services.AddScoped<UnitOfWork<MainDbContext>>();

        return services;
    }
}
