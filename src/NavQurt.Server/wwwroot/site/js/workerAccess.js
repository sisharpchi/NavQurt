document.addEventListener('DOMContentLoaded', function () {
    const workerTypeSelect = document.getElementById('InputModel_WorkerType');

    // Маппинг рекомендуемых прав по ролям
    const recommendedAccess = {

        "0": // официант
            [
                "CanOpenShift",
                "CanPrintOrder",
                "CanMoveOrderItemsToAnotherOrder",
                "CanSwapTable",
            ],
        "1": // кассир
            [
                "CanCloseShift",
                "CanOpenShift",
                "CanRemoveOrderItem",
                "CanPrintOrder",
                "CanCancelOrder",
                "CanCompleteOrder",
                "CanSwapTable",
                "CanEditOrderOpenedNotMyself",
                "CanPrintOrCancelOrCompleteSomeoneOrder",
                "CanChangeServicePercentage",
                "CanRestoreOrder",
                "CanCreateOrderWithDebt",
                "CanModifyOrderBeignOrderLocked",
                "CanMakeAsReadyOrder",
                "CanModifyPayedDebt",
                "CanSetDiscount",
                "CanOperateWithRahmat",
                "TransactionsAccess",
                "CanOperateWithPreOrder",
                "CanSplitOrderItems",
                "CanOperateWithPrepayments",
                "ControlGuestCount",
                "CanChangeMenu",
                "CanStopService",
                "CanStartService",
                "CanSetService",
                "CanCancelOrRestoreService",
                "HasOperationCommandsAccess",
                "CanCloseOrOpenZReport",
                "CanChangeWaiter",
                "CanViewShiftData",
                "CanUnionOrders",
                "CanSeperateOrders",
                "CanSplitOrderBetweenGuests",
                "CanMoveOrderItemsToAnotherOrder",
                "CanWorkWithStopList",
                "CanAccessTakeAway",
                "CanCancelRahmatPayment",
            ],
        "2": // админ
            [
                "CanCloseShift",
                "CanOpenShift",
                "CanRemoveOrderItem",
                "CanPrintOrder",
                "CanCancelOrder",
                "CanCompleteOrder",
                "CanSwapTable",
                "CanEditOrderOpenedNotMyself",
                "CanPrintOrCancelOrCompleteSomeoneOrder",
                "CanChangeServicePercentage",
                "CanSeeReport",
                "CanRestoreOrder",
                "CanCreateOrderWithDebt",
                "CanModifyOrderBeignOrderLocked",
                "CanSeeSetting",
                "HaveAccessToWorkers",
                "HaveAccessToOrderTypePercent",
                "CanMakeAsReadyOrder",
                "CanModifyPayedDebt",
                "CanSetDiscount",
                "CanOperateWithRahmat",
                "TransactionsAccess",
                "CanOperateWithPreOrder",
                "CanSplitOrderItems",
                "CanOperateWithPrepayments",
                "ControlGuestCount",
                "CanChangeMenu",
                "CanStopService",
                "CanStartService",
                "CanSetService",
                "CanCancelOrRestoreService",
                "HasOperationCommandsAccess",
                "CanCloseOrOpenZReport",
                "CanCloseApplication",
                "CanChangeWaiter",
                "CanViewShiftData",
                "CanUnionOrders",
                "CanSeperateOrders",
                "CanSplitOrderBetweenGuests",
                "CanMoveOrderItemsToAnotherOrder",
                "CanWorkWithStopList",
                "CanAccessTakeAway",
                "CanCancelRahmatPayment",
            ],

        "4": [],// курьер
    };

    workerTypeSelect.addEventListener('change', function () {
        const selected = this.value;

        // Сначала сбросить все галки
        document.querySelectorAll('input[type="checkbox"][name^="InputModel"]').forEach(chk => {
            chk.checked = false;
        });

        // Установить рекомендуемые для выбранного типа
        if (recommendedAccess[selected]) {
            recommendedAccess[selected].forEach(accessName => {
                const checkbox = document.querySelector(`input[name="InputModel.${accessName}"]`);
                if (checkbox) {
                    checkbox.checked = true;
                }
            });
        }
    });
});
