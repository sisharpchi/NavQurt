using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NavQurt.Infrastructure.Helpers;

namespace NavQurt.Infrastructure.Data;

public class MainDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
{
    public MainDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("MainDatabase")
            ?? ConnectionStrings.LocalMain;

        var options = new DbContextOptionsBuilder<MainDbContext>();
        DbContextHelpers.ConfigurePostgreSql(connectionString, options);

        return new MainDbContext(options.Options);
    }
}
