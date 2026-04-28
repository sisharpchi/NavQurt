const incomeCreated = "IncomeCreated";
const incomeCanceled = "IncomeCanceled";

const inventoryCanceled = "InventoryCanceled";
const inventoryCreated = "InventoryCreated";
const inventoryUpdated = "InventoryUpdated";

const ingredientWriteOffCreated = "IngredientWriteoffCreated";
const ingredientWriteOffCanceled = "IngredientWriteoffCanceled";

const transactionCreated = "TransactionCreated";
const transactionCanceled = "TransactionCanceled";
const transactionUpdated = "TransactionUpdated";
const transactionShiftUpdated = "TransactionShiftUpdated";

const semiProductAccepted = "SemiProductAccepted";
const semiProductCanceled = "SemiProductCanceled";


const movementAccepted = "AcceptMovement";
const movementCanceled = "CancelMovement";

connection = new signalR.HubConnectionBuilder()
    .withUrl("/siteHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();


async function startSignalR(
    incomeCreatedEvent = {},
    incomeCanceledEvent = {},
    inventoryCreatedEvent = {},
    inventroyUpdatedEvent = {},
    inventoryCanceledEvent = {}) {
    try {
        connection.on(incomeCreated, incomeCreatedEvent);
        connection.on(incomeCanceled, incomeCanceledEvent);

        connection.on(inventoryCanceled, inventoryCanceledEvent);
        connection.on(inventoryCreated, inventoryCreatedEvent);
        connection.on(inventoryUpdated, inventroyUpdatedEvent);

        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(startSignalR, 5000);
    }
};

function WriteOffCreated(ingredientWriteOffCreatedEvent = {}) {
    connection.on(ingredientWriteOffCreated, ingredientWriteOffCreatedEvent);
}
function WriteOffCanceled(callback = {}) {
    connection.on(ingredientWriteOffCanceled, callback);
}

function movementAcceptedEvent(callback = {}) {
    connection.on(movementAccepted, callback);
}
function movementCanceledEvent(callback = {}) {
    connection.on(movementCanceled, callback);
}

function transactionCreatedEvent(callback = {}) {
    connection.on(transactionCreated, callback);
}
function transactionCanceledEvent(callback = {}) {
    connection.on(transactionCanceled, callback);
}

function shiftTransactionUpdatedEvent(callback = {}) {
    connection.on(transactionShiftUpdated, callback);
}

function semiProductAcceptedEvent(callback = {}) {
    connection.on(semiProductAccepted, callback);
}

function semiProductCanceledEvent(callback = {}) {
    connection.on(semiProductCanceled, callback);
}
