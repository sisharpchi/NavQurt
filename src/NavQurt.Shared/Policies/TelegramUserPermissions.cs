namespace NavQurt.Shared.Policies;

public static class TelegramUserPermissions
{
    public const string OrderCanceled = "TelegramUser.CancelOrder";
    public const string OrderCompleted = "TelegramUser.OrderCompleted";
    public const string OrderItemRemoved = "TelegramUser.OrderItemRemoved";
    public const string IncludeProductsNameWhenorderCompleted = "TelegramUser.IncludeProductsNameWhenorderCompleted";
    public const string ShiftClosed = "TelegramUser.ShiftClosed";
    public const string CompanyConnectionState = "TelegramUser.CompanyConnectionState";
    public const string ExternalOrderState = "TelegramUser.ExternalOrderState";

    public static HashSet<string> All =>
    [
        OrderCanceled,
        OrderCompleted,
        OrderItemRemoved,
        IncludeProductsNameWhenorderCompleted,
        ShiftClosed,
        CompanyConnectionState,
        ExternalOrderState
    ];
}
