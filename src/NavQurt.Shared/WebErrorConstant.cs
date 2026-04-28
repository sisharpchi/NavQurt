namespace NavQurt.Shared;

public static class WebErrorConstant
{
    public const int UnknownError = 1;
    public const int NotFound = 2;
    public const int IsNotCreatedStatus = 3;
    public const int AlreadyMovemented = 4;
    public const int AlreadyAccepted = 5;
    public const int InvalidDate = 6;

    public const int InvalidPatch = 10;
    public const int DuplicateCompanyEntry = 11;
    public const int CompanyNotFoundOrNotInOrganization = 12;
    public const int CompanyNotExists = 13;
    public const int CompanyOffline = 14;
    public const int UserNotExists = 15;
    public const int UserNotAttachedToCompany = 16;
    public const int SmsSendingFailed = 17;
    public const int WorkerAlreadyAttachedToAnotherUser = 18;
    public const int UserClaimsRemoveFailed = 19;
    public const int InvalidOrNotAllowedClaims = 20;
    public const int UserClaimsAddFailed = 21;

    public const int TitleRequired = 40;
    public const int AccountNotExists = 41;
    public const int IsMain = 42;
    public const int HasBalance = 43;

    public const int DidoxNotExists = 50;
    public const int DidoxAlreadyExists = 51;

    public const int ProductNotExists = 60;
    public const int FavouriteProductNotExists = 61;

    public const int IncomeNotExists = 70;
    public const int IncomeAlreadyCanceled = 71;
    public const int IncomeHasMovementsAfterDate = 72;
    public const int WarehouseNotExists = 73;
    public const int ProviderNotExists = 74;
    public const int IngredientNotFoundAfterValidation = 75;
    public const int StatusNotUpdatable = 76;
    public const int AlreadyCanceled = 77;
    public const int ProviderPaymentDateExceededIncomeDate = 78;
    public const int UnableSetProviderIdToNullThereAreProviderPayments = 79;
    public const int PaymentExceedsInvoiceAmount = 80;

    public const int WarehouseProductInUse = 90;
    public const int WarehouseCannotBeDeletedInCurrentVersion = 91;
    public const int WarehouseBelongsToCompany = 92;

    public const int NoAccessibleWorkerForCompany = 100;
    public const int WorkerCodeAlreadyExists = 101;
    public const int WorkerSwipeCodeAlreadyExists = 102;

    public const int IngredientNotExists = 110;
    public const int IngredientCategoryTitleAlreadyExists = 111;
    public const int IngredientCategoryNotExists = 112;
    public const int CategoryIsEmpty = 113;
    public const int IngredientTitleAlreadyExists = 114;
    public const int IngredientInUse = 115;

    public const int PackingNotExists = 120;
    public const int UnitTypeTitleExists = 130;
    public const int UnitTypeNotExists = 131;
    public const int SystemUnitTypeCannotBeChanged = 132;

    public const int IngredientWriteoffNotExists = 140;
    public const int IngredientWriteoffAlreadyCanceled = 141;
    public const int IngredientWriteoffInvalidStatusForCancel = 142;
    public const int IngredientWarehouseNotExists = 150;
    public const int InsufficientIngredientStock = 151;

    public const int MovementBetweenCompaniesNotAllowed = 160;
    public const int MovementSameWarehouseNotAllowed = 161;
    public const int MovementNotExists = 162;
    public const int MovementHasNoCompanyWarehouse = 163;

    public const int PaymentMethodTitleExists = 170;
    public const int AccountNotLinkedToCompany = 171;
    public const int PaymentMethodNotExists = 172;
    public const int PaymentMethodNotDeletable = 173;
    public const int PaymentTypeAccountTypeMismatch = 174;

    public const int PaymentTransactionCategoryTitleExists = 180;
    public const int PaymentTransactionCategoryNotExists = 181;
    public const int PrinterRoleMust = 190;
    public const int PrinterPlaceNotExists = 191;
    public const int PrinterRoleNotExists = 200;
    public const int PrinterRoleTitleExists = 201;
    public const int PrinterRoleInUseByActiveProducts = 202;

    public const int ProductCategoryTitleAlreadyExists = 210;
    public const int ProductBarCodePrinterAlreadyExists = 211;
    public const int ProductTitleAlreadyExists = 212;
    public const int ProductCategoryNotExists = 213;
    public const int UnsupportedImage = 214;
    public const int WarehouseIsRequired = 215;
    public const int ProductCategoryCannotBeOwnParent = 216;
    public const int ProductCategoryHasChildren = 217;

    public const int RecipeNotExists = 220;
    public const int RecipeAlreadyCompleted = 221;
    public const int RecipeInvalidReference = 222;
    public const int RecipeBothProductAndIngredientProvided = 223;
    public const int RecipeInvalidIngredientType = 224;
    public const int RecipeInvalidPortion = 225;
    public const int RecipeDuplicateIngredient = 226;
    public const int RecipeSelfReference = 227;

    public const int WarehousesMustBelongToSameCompany = 230;
    public const int SemiProductCreationNotExists = 231;
    public const int SemiProductCreationInvalidStatus = 232;
    public const int SemiProductCreationInvalidIngredient = 233;
    public const int SemiProductCreationContainerAlreadyCanceled = 234;

    public const int ShiftNotExists = 240;
    public const int TransactionNotExists = 250;
    public const int TransactionCategoryNotExists = 251;
    public const int TransactionCategoryItemNotExists = 253;
    public const int TitleAlreadyExists = 254;
    public const int TransactionAlreadyCanceled = 255;
    public const int InvalidTransactionDate = 256;
    public const int ShiftEntityCannotBeUpdated = 257;
    public const int ShiftEntityCannotBeCanceled = 258;

    public const int InventoryNotExists = 260;
    public const int InventoryInvalidStatusForEditing = 261;
    public const int InventoryAlreadyCanceled = 262;
    public const int InventoryInvalidStatusForUpdate = 263;
    public const int InventoryInvalidStatusForItemCreation = 264;
    public const int InventoryIngredientIdsNotProvided = 265;
    public const int InventoryIngredientsNotFound = 266;
    public const int InventoryHasCompanyWarehouse = 267;
    public const int InventoryIsNotEditingStatus = 268;
    public const int ProductAttributeNotExists = 269;
    public const int AlreadyUpdated = 270;

    public const int TelegramUserAlreadyExists = 280;
    public const int TelegramUserNotFound = 281;
    public const int TelegramUserDoesntExists = 282;
    public const int CurrentTelegramUserAlreadyBelongToCurrentCompany = 283;

    public const int NotSupported = 300;
    public const int UnableAttachProductToMPProductItHasAlreaadyBeenAttached = 400;

    public const int IntegrationNotFound = 410;
    public const int WebhookNotFound = 411;
    public const int InvalidWebhookUrl = 412;
    public const int MenuBindingNotFound = 413;
    public const int NoMenuConfigured = 414;
    public const int MarketPlaceGroupNotFound = 415;
    public const int MarketPlaceProductNotFound = 416;

    public const int DisassemblyNotExists = 417;
    public const int DisassemblyInvalidStatus = 418;
    public const int DisassemblyWarehouseNotFound = 419;
    public const int DisassemblyWarehouseNotBelongToCompany = 420;
    public const int DisassemblyInvalidDistributionPercent = 421;
    public const int DisassemblySourceInOutputs = 422;
    public const int DisassemblyIngredientWarehouseNotFound = 423;
    public const int DisassemblyInvalidIngredientType = 424;

    public const int MxikFetchFailed = 430;
    public const int MxikDataNotFound = 431;
    public const int MxikInvalidPackageCode = 432;
}
