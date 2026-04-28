using System.Security.Claims;

namespace NavQurt.Shared.Claims;

public static class ClaimsStore
{
    public static readonly List<ClaimRepresent> Dashboard =
    [
        new("Dashboard.Main", new Claim("Permission", "Permission.Dashboard")),
    ];

    public static readonly List<ClaimRepresent> Menu =
    [
        new("Menu.Section", new Claim("Permission", "Permission.Menu")),
        new("Category.View", new Claim("Permission", "Permission.Category.CanView")),
        new("Category.Modify", new Claim("Permission", "Permission.Category.CanModify")),
        new("Product.View", new Claim("Permission", "Permission.Product.CanView")),
        new("Product.Modify", new Claim("Permission", "Permission.Product.CanModify")),
        new("IngredientCategory.View", new Claim("Permission", "Permission.IngredientCategory.CanView")),
        new("IngredientCategory.Modify", new Claim("Permission", "Permission.IngredientCategory.CanModify")),
        new("Ingredient.View", new Claim("Permission", "Permission.Ingredient.CanView")),
        new("Ingredient.Modify", new Claim("Permission", "Permission.Ingredient.CanModify")),
        new("UnitType.View", new Claim("Permission", "Permission.UnitType.CanView")),
        new("UnitType.Modify", new Claim("Permission", "Permission.UnitType.CanModify")),
        new("PaymentMethod.View", new Claim("Permission", "Permission.PaymentMethod.CanView")),
        new("PaymentMethod.Modify", new Claim("Permission", "Permission.PaymentMethod.CanModify")),
        new("Recipe.Access", new Claim("Permission", "Permission.Recipe")),
        new("QrMenu.Access", new Claim("Permission", "Permission.QrMenu")),
        new("FavouriteProduct.View", new Claim("Permission", "Permission.FavouriteProduct.CanView")),
        new("FavouriteProduct.Modify", new Claim("Permission", "Permission.FavouriteProduct.CanModify")),
        new("Printer.View", new Claim("Permission", "Permission.Printer.CanView")),
        new("Printer.Modify", new Claim("Permission", "Permission.Printer.CanModify")),
    ];

    public static readonly List<ClaimRepresent> Warehouse =
    [
        new("Warehouse.Section", new Claim("Permission", "Permission.Warehouse")),
        new("Warehouse.Remainder", new Claim("Permission", "Permission.WarehouseRemainder")),
        new("Income.View", new Claim("Permission", "Permission.Income.CanView")),
        new("Income.Modify", new Claim("Permission", "Permission.Income.CanModify")),
        new("Income.Cancel", new Claim("Permission", "Permission.Income.CanCancel")),
        new("Income.Restore", new Claim("Permission", "Permission.Income.CanRestore")),
        new("Movement.View", new Claim("Permission", "Permission.Movement.CanView")),
        new("Movement.Modify", new Claim("Permission", "Permission.Movement.CanModify")),
        new("Movement.Cancel", new Claim("Permission", "Permission.Movement.CanCancel")),
        new("Writeoff.View", new Claim("Permission", "Permission.Writeoff.CanView")),
        new("Writeoff.Modify", new Claim("Permission", "Permission.Writeoff.CanModify")),
        new("Writeoff.Cancel", new Claim("Permission", "Permission.Writeoff.CanCancel")),
        new("Writeoff.Restore", new Claim("Permission", "Permission.Writeoff.CanRestore")),
        new("Inventory.View", new Claim("Permission", "Permission.Inventory.CanView")),
        new("Inventory.Modify", new Claim("Permission", "Permission.Inventory.CanModify")),
        new("Inventory.Cancel", new Claim("Permission", "Permission.Inventory.CanCancel")),
        new("Inventory.Restore", new Claim("Permission", "Permission.Inventory.CanRestore")),
        new("SemiProduct.View", new Claim("Permission", "Permission.SemiProduct.CanView")),
        new("SemiProduct.Modify", new Claim("Permission", "Permission.SemiProduct.CanModify")),
        new("SemiProduct.Cancel", new Claim("Permission", "Permission.SemiProduct.CanCancel")),
        new("Disassembly.View", new Claim("Permission", "Permission.Disassembly.CanView")),
        new("Disassembly.Modify", new Claim("Permission", "Permission.Disassembly.CanModify")),
        new("Disassembly.Cancel", new Claim("Permission", "Permission.Disassembly.CanCancel")),
        new("Provider.View", new Claim("Permission", "Permission.Provider.CanView")),
        new("Provider.Modify", new Claim("Permission", "Permission.Provider.CanModify")),
        new("Warehouses.View", new Claim("Permission", "Permission.Warehouses.CanView")),
        new("Warehouses.Modify", new Claim("Permission", "Permission.Warehouses.CanModify")),
        new("ProviderPayment.View", new Claim("Permission", "Permission.ProviderPayment.CanView")),
        new("ProviderPayment.Modify", new Claim("Permission", "Permission.ProviderPayment.CanModify")),
        new("ProviderPayment.Delete", new Claim("Permission", "Permission.ProviderPayment.CanDelete")),
        new("ProviderWork.View", new Claim("Permission", "Permission.ProviderWork.CanView")),
        new("ProviderBalance.View", new Claim("Permission", "Permission.ProviderBalance.CanView")),
    ];

    public static readonly List<ClaimRepresent> Stats =
    [
        new("Stats.Section", new Claim("Permission", "Permission.Stat")),
        new("Stats.Overall", new Claim("Permission", "Permission.Stat.Overall")),
        new("Stats.Sales", new Claim("Permission", "Permission.Stat.Sales")),
        new("Stats.PerDays", new Claim("Permission", "Permission.Stat.PerDays")),
        new("Stats.Orders", new Claim("Permission", "Permission.Stat.Orders")),
        new("Stats.Category", new Claim("Permission", "Permission.Stat.Category")),
        new("Stats.Table", new Claim("Permission", "Permission.Stat.Table")),
        new("Stats.Canceled", new Claim("Permission", "Permission.Stat.Canceled")),
        new("Stats.PaymentMethod", new Claim("Permission", "Permission.Stat.PaymentMethod")),
        new("Stats.Worker", new Claim("Permission", "Permission.Stat.Worker")),
        new("Stats.Courier", new Claim("Permission", "Permission.Stat.Courier")),
        new("Stats.IngredientMovement", new Claim("Permission", "Permission.Stat.IngredientMovement")),
        new("Stats.TransactionOutcomes", new Claim("Permission", "Permission.Stat.TransactionOutcomes")),
        new("Stats.Service", new Claim("Permission", "Permission.Stat.Service")),
        new("Stats.Debt", new Claim("Permission", "Permission.Stat.Debt")),
        new("Stats.ABCXYZ", new Claim("Permission", "Permission.Stat.ABCXYZ")),
    ];

    public static readonly List<ClaimRepresent> Finance =
    [
        new("Finance.Section", new Claim("Permission", "Permission.Finance")),
        new("Finance.Modify", new Claim("Permission", "Permission.Finance.CanModify")),
        new("Transaction.View", new Claim("Permission", "Permission.Transaction.CanView")),
        new("Transaction.Create", new Claim("Permission", "Permission.Transaction.CanCreate")),
        new("Transaction.Update", new Claim("Permission", "Permission.Transaction.CanUpdate")),
        new("Transaction.Cancel", new Claim("Permission", "Permission.Transaction.CanCancel")),
    ];

    public static readonly List<ClaimRepresent> AuditHistory =
    [
        new("AuditHistory.Section", new Claim("Permission", "Permission.AuditHistory")),
        new("ProductPrice.View", new Claim("Permission", "Permission.ProductPrice.CanView")),
    ];

    public static readonly List<ClaimRepresent> Workers =
    [
        new("Workers.View", new Claim("Permission", "Permission.Worker.CanView")),
        new("Workers.Modify", new Claim("Permission", "Permission.Worker.CanModify")),
    ];

    public static readonly List<ClaimRepresent> MyUsers =
    [
        new("Users.Section", new Claim("Permission", "Permission.Users")),
        new("Users.TgSection", new Claim("Permission", "Permission.TgUsers")),
        new("Setting.Terminal", new Claim("Permission", "Permission.Terminals")),
    ];

    public static readonly List<ClaimRepresent> Integrations =
    [
        new("Integrations.Section", new Claim("Permission", "Permission.Integrations")),
        new("Integrations.View", new Claim("Permission", "Permission.Integrations.CanView")),
        new("Integrations.Modify", new Claim("Permission", "Permission.Integrations.CanModify")),
        new("MarketPlace.View", new Claim("Permission", "Permission.MarketPlace.CanView")),
        new("MarketPlace.Modify", new Claim("Permission", "Permission.MarketPlace.CanModify")),
    ];

    private static readonly Lazy<IReadOnlyCollection<ClaimRepresent>> Claims = new(() =>
    [
        .. Dashboard,
        .. Menu,
        .. Warehouse,
        .. Stats,
        .. Finance,
        .. Workers,
        .. MyUsers,
        .. AuditHistory,
        .. Integrations
    ]);

    public static IReadOnlyCollection<ClaimRepresent> AllClaims => Claims.Value;
}

public class ClaimRepresent
{
    public ClaimRepresent(string title, Claim claim)
    {
        Title = title;
        Claim = claim;
    }

    public string Title { get; set; }
    public Claim Claim { get; set; }
}
