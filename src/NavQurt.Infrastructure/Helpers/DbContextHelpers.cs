using Microsoft.EntityFrameworkCore;

namespace NavQurt.Infrastructure.Helpers;

internal static class DbContextHelpers
{
    public static void ConfigurePostgreSql(string connectionString, DbContextOptionsBuilder options)
    {
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: ["08P01", "08006", "08001", "08004"]);
        });
    }
}
