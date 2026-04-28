using System.Reflection;

namespace NavQurt.Shared.Policies;

public static class WorkerPermissions
{
    public static class Order
    {
        public const string CanComplete = nameof(Order) + "." + nameof(CanComplete);
        public const string CanCancel = nameof(Order) + "." + nameof(CanCancel);
        public const string CanPrint = nameof(Order) + "." + nameof(CanPrint);
        public const string CanMakeAsReadyOrder = nameof(Order) + "." + nameof(CanMakeAsReadyOrder);
        public const string CanRemoveOrderItem = nameof(Order) + "." + nameof(CanRemoveOrderItem);
        public const string CanSwapTable = nameof(Order) + "." + nameof(CanSwapTable);
        public const string CanEditOrderOpenedNotMyself = nameof(Order) + "." + nameof(CanEditOrderOpenedNotMyself);
        public const string CanPrintOrCancelOrCompleteSomeoneOrder = nameof(Order) + "." + nameof(CanPrintOrCancelOrCompleteSomeoneOrder);
        public const string CanChangeServicePercentage = nameof(Order) + "." + nameof(CanChangeServicePercentage);
        public const string CanCreateOrderWithDebt = nameof(Order) + "." + nameof(CanCreateOrderWithDebt);
        public const string CanModifyOrderBeignOrderLocked = nameof(Order) + "." + nameof(CanModifyOrderBeignOrderLocked);
        public const string CanRestoreOrder = nameof(Order) + "." + nameof(CanRestoreOrder);
        public const string CanSetDiscount = nameof(Order) + "." + nameof(CanSetDiscount);
        public const string CanOperateWithRahmat = nameof(Order) + "." + nameof(CanOperateWithRahmat);
        public const string CanOperateWithPreOrder = nameof(Order) + "." + nameof(CanOperateWithPreOrder);
        public const string CanSplitOrderItems = nameof(Order) + "." + nameof(CanSplitOrderItems);
        public const string CanMoveOrderItemsToAnotherOrder = nameof(Order) + "." + nameof(CanMoveOrderItemsToAnotherOrder);
        public const string CanSplitOrderBetweenGuests = nameof(Order) + "." + nameof(CanSplitOrderBetweenGuests);
        public const string CanUnionOrders = nameof(Order) + "." + nameof(CanUnionOrders);
        public const string CanSeperateOrders = nameof(Order) + "." + nameof(CanSeperateOrders);
        public const string CanOperateWithPrepayments = nameof(Order) + "." + nameof(CanOperateWithPrepayments);
        public const string ControlGuestCount = nameof(Order) + "." + nameof(ControlGuestCount);
        public const string CanChangeMenu = nameof(Order) + "." + nameof(CanChangeMenu);
        public const string CanStopService = nameof(Order) + "." + nameof(CanStopService);
        public const string CanStartService = nameof(Order) + "." + nameof(CanStartService);
        public const string CanSetService = nameof(Order) + "." + nameof(CanSetService);
        public const string CanCancelOrRestoreService = nameof(Order) + "." + nameof(CanCancelOrRestoreService);
        public const string CanChangeWaiter = nameof(Order) + "." + nameof(CanChangeWaiter);
        public const string CanCancelRahmatPayment = nameof(Order) + "." + nameof(CanCancelRahmatPayment);
        public const string CanWorkWithStopList = nameof(Order) + "." + nameof(CanWorkWithStopList);
        public const string CanAccessTakeAway = nameof(Order) + "." + nameof(CanAccessTakeAway);
        public const string CanWorkerWithReturnOrder = nameof(Order) + "." + nameof(CanWorkerWithReturnOrder);
        public const string CanWorkerWithTakeAwayInTable = nameof(Order) + "." + nameof(CanWorkerWithTakeAwayInTable);
        public const string CanChangeClosedOrderPayment = nameof(Order) + "." + nameof(CanChangeClosedOrderPayment);
        public const string CanSendSms = nameof(Order) + "." + nameof(CanSendSms);
        public const string CanMangeProductLabel = nameof(Order) + "." + nameof(CanMangeProductLabel);
    }

    public static class Shift
    {
        public const string CanOpen = nameof(Shift) + "." + nameof(CanOpen);
        public const string CanClose = nameof(Shift) + "." + nameof(CanClose);
        public const string CanCloseOrOpenZReport = nameof(Shift) + "." + nameof(CanCloseOrOpenZReport);
        public const string CanViewShiftData = nameof(Shift) + "." + nameof(CanViewShiftData);
    }

    public static class Other
    {
        public const string CanSeeReport = nameof(Other) + "." + nameof(CanSeeReport);
        public const string CanSeeSetting = nameof(Other) + "." + nameof(CanSeeSetting);
        public const string HaveAccessToWorkers = nameof(Other) + "." + nameof(HaveAccessToWorkers);
        public const string HaveAccessToOrderTypePercent = nameof(Other) + "." + nameof(HaveAccessToOrderTypePercent);
        public const string TransactionsAccess = nameof(Other) + "." + nameof(TransactionsAccess);
        public const string HasOperationCommandsAccess = nameof(Other) + "." + nameof(HasOperationCommandsAccess);
        public const string CanCloseApplication = nameof(Other) + "." + nameof(CanCloseApplication);
        public const string CanModifyPayedDebt = nameof(Order) + "." + nameof(CanModifyPayedDebt);
    }

    public static IEnumerable<string> All() =>
        typeof(WorkerPermissions)
            .GetNestedTypes()
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .Select(f => (string)f.GetValue(null)!);

    public static HashSet<string> AdminPersmissions = All().ToHashSet();

    public static HashSet<string> CashierPersmissions =
    [
        Shift.CanOpen,
        Shift.CanClose,
        Shift.CanViewShiftData,
        Shift.CanCloseOrOpenZReport,
        Order.CanCancel,
        Order.CanComplete,
        Order.CanPrint,
        Order.CanRemoveOrderItem,
        Order.CanSwapTable,
        Order.CanEditOrderOpenedNotMyself,
        Order.CanChangeServicePercentage,
        Order.CanMakeAsReadyOrder,
        Order.CanSetDiscount,
        Order.CanModifyOrderBeignOrderLocked,
        Order.CanRestoreOrder,
        Order.CanCreateOrderWithDebt,
        Order.CanOperateWithPreOrder,
        Order.ControlGuestCount,
        Order.CanCancelOrRestoreService,
        Order.CanChangeMenu,
        Order.CanOperateWithRahmat,
        Order.CanOperateWithPrepayments,
        Order.CanSetService,
        Order.CanSplitOrderItems,
        Order.CanStartService,
        Order.CanStopService,
        Order.CanChangeWaiter,
        Order.CanMoveOrderItemsToAnotherOrder,
        Order.CanSeperateOrders,
        Order.CanUnionOrders,
        Order.CanSplitOrderBetweenGuests,
        Order.CanCancelRahmatPayment,
        Order.CanPrintOrCancelOrCompleteSomeoneOrder,
        Order.CanWorkWithStopList,
        Order.CanAccessTakeAway,
        Order.CanWorkerWithReturnOrder,
        Order.CanWorkerWithTakeAwayInTable,
        Other.TransactionsAccess,
        Other.CanModifyPayedDebt,
        Other.HasOperationCommandsAccess
    ];

    public static HashSet<string> WaiterPersmissions =
    [
        Shift.CanOpen,
        Order.CanPrint,
        Order.CanMoveOrderItemsToAnotherOrder,
        Order.CanSwapTable
    ];
}
