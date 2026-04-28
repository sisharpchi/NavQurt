using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NavQurt.Core.Entities;

namespace NavQurt.Infrastructure.Data;

public class MainDbContext(DbContextOptions<MainDbContext> options) : IdentityDbContext<User, AppRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<AppRole>().ToTable("Roles");

        modelBuilder.Entity<OpenIdApplication>().ToTable("OpenIddictEntityFrameworkCoreApplications");
        modelBuilder.Entity<OpenIdAuthorization>().ToTable("OpenIddictEntityFrameworkCoreAuthorizations");
        modelBuilder.Entity<OpenIdScope>().ToTable("OpenIddictEntityFrameworkCoreScopes");
        modelBuilder.Entity<OpenIdToken>().ToTable("OpenIddictEntityFrameworkCoreTokens");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MainDbContext).Assembly);
    }
}
