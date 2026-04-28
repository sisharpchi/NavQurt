using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NavQurt.Core.Entities;
using NavQurt.Core.Entities.Business;

namespace NavQurt.Infrastructure.Data;

public class MainDbContext(DbContextOptions<MainDbContext> options) : IdentityDbContext<User, AppRole, string>(options)
{
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductComboItem> ProductComboItems => Set<ProductComboItem>();
    public DbSet<IngredientCategory> IngredientCategories => Set<IngredientCategory>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeItem> RecipeItems => Set<RecipeItem>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<IngredientStock> IngredientStocks => Set<IngredientStock>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Income> Incomes => Set<Income>();
    public DbSet<IncomeItem> IncomeItems => Set<IncomeItem>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Worker> Workers => Set<Worker>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderPayment> OrderPayments => Set<OrderPayment>();

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

        ConfigureBusinessEntities(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MainDbContext).Assembly);
    }

    private static void ConfigureBusinessEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => x.Title).IsUnique();
            entity.HasOne(x => x.ParentCategory)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.Property(x => x.PhotoPath).HasMaxLength(512);
            entity.Property(x => x.Price).HasPrecision(18, 2);
            entity.Property(x => x.SelfPrice).HasPrecision(18, 2);
            entity.HasIndex(x => x.Title).IsUnique();
            entity.HasOne(x => x.ProductCategory)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.ProductCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ProductComboItem>(entity =>
        {
            entity.Property(x => x.Quantity).HasPrecision(18, 3);
            entity.HasIndex(x => new { x.ComboProductId, x.ProductId }).IsUnique();
            entity.HasOne(x => x.ComboProduct)
                .WithMany(x => x.ComboItems)
                .HasForeignKey(x => x.ComboProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Product)
                .WithMany(x => x.IncludedInCombos)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<IngredientCategory>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => x.Title).IsUnique();
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Unit).HasMaxLength(32).IsRequired();
            entity.Property(x => x.MinRemainderLimit).HasPrecision(18, 3);
            entity.Property(x => x.AverageSelfPrice).HasPrecision(18, 2);
            entity.HasIndex(x => x.Title).IsUnique();
            entity.HasOne(x => x.IngredientCategory)
                .WithMany(x => x.Ingredients)
                .HasForeignKey(x => x.IngredientCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.Property(x => x.PortionYield).HasPrecision(18, 3);
            entity.HasIndex(x => x.ProductId).IsUnique();
            entity.HasIndex(x => x.IngredientId).IsUnique();
            entity.ToTable(x => x.HasCheckConstraint("CK_Recipes_ProductOrIngredient", "(\"ProductId\" IS NULL) <> (\"IngredientId\" IS NULL)"));
            entity.HasOne(x => x.Product)
                .WithOne(x => x.Recipe)
                .HasForeignKey<Recipe>(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Ingredient)
                .WithOne(x => x.Recipe)
                .HasForeignKey<Recipe>(x => x.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RecipeItem>(entity =>
        {
            entity.Property(x => x.Quantity).HasPrecision(18, 3);
            entity.HasIndex(x => new { x.RecipeId, x.IngredientId }).IsUnique();
            entity.HasOne(x => x.Recipe)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Ingredient)
                .WithMany(x => x.RecipeItems)
                .HasForeignKey(x => x.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => x.Title).IsUnique();
            entity.HasData(new Warehouse { Id = 1, Title = "Main", IsMain = true, IsActive = true });
        });

        modelBuilder.Entity<IngredientStock>(entity =>
        {
            entity.Property(x => x.Quantity).HasPrecision(18, 3);
            entity.HasIndex(x => new { x.WarehouseId, x.IngredientId }).IsUnique();
            entity.HasOne(x => x.Warehouse)
                .WithMany(x => x.IngredientStocks)
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Ingredient)
                .WithMany(x => x.Stocks)
                .HasForeignKey(x => x.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.Property(x => x.Quantity).HasPrecision(18, 3);
            entity.Property(x => x.BalanceAfter).HasPrecision(18, 3);
            entity.Property(x => x.Comment).HasMaxLength(500);
        });

        modelBuilder.Entity<Income>(entity =>
        {
            entity.Property(x => x.TotalAmount).HasPrecision(18, 2);
            entity.Property(x => x.Comment).HasMaxLength(500);
        });

        modelBuilder.Entity<IncomeItem>(entity =>
        {
            entity.Property(x => x.Quantity).HasPrecision(18, 3);
            entity.Property(x => x.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => x.Title).IsUnique();
            entity.HasData(
                new PaymentMethod { Id = 1, Title = "Cash", IsActive = true },
                new PaymentMethod { Id = 2, Title = "Card", IsActive = true },
                new PaymentMethod { Id = 3, Title = "Click/Payme", IsActive = true });
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.PhoneNumber).HasMaxLength(32).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Location).HasMaxLength(512);
            entity.HasIndex(x => x.PhoneNumber).IsUnique();
        });

        modelBuilder.Entity<Worker>(entity =>
        {
            entity.Property(x => x.FullName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.PhoneNumber).HasMaxLength(32);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(x => x.OrderNumber).HasMaxLength(32).IsRequired();
            entity.Property(x => x.TotalAmount).HasPrecision(18, 2);
            entity.Property(x => x.CustomerPhoneNumber).HasMaxLength(32).IsRequired();
            entity.Property(x => x.CustomerFullName).HasMaxLength(160).IsRequired();
            entity.Property(x => x.CustomerLocation).HasMaxLength(512);
            entity.Property(x => x.Comment).HasMaxLength(500);
            entity.HasIndex(x => x.OrderNumber).IsUnique();
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(x => x.ProductTitle).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Quantity).HasPrecision(18, 3);
            entity.Property(x => x.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<OrderPayment>(entity =>
        {
            entity.Property(x => x.Amount).HasPrecision(18, 2);
        });
    }
}
