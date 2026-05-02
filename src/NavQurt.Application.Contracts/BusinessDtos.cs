namespace NavQurt.Application.Contracts;

public record ListResponse<T>(IReadOnlyCollection<T> Items, int ItemsCount);
public record EntityListRequest(string? Search = null, bool? IsActive = null);

public record ProductCategoryDto(int Id, string Title, int? ParentCategoryId, bool IsActive, int Priority);
public record ProductCategoryListRequest(string? Search = null, bool? IsActive = null, int? ParentCategoryId = null);
public record ProductCategoryRequest(string Title, int? ParentCategoryId, bool IsActive = true, int Priority = 0);

public record ProductDto(
    int Id,
    string Title,
    string? Description,
    decimal Price,
    decimal SelfPrice,
    bool IsActive,
    bool IsCombo,
    bool UseOwnRecipeForCombo,
    int? ProductCategoryId,
    string? ProductCategoryTitle,
    int? RecipeId,
    bool HasRecipe,
    IReadOnlyCollection<ProductComboItemDto> ComboItems);

public record ProductListRequest(string? Search = null, int? CategoryId = null, bool? IsActive = null, bool? IsCombo = null, bool OnlyWithoutRecipe = false);
public record ProductRequest(
    string Title,
    string? Description,
    decimal Price,
    decimal SelfPrice,
    bool IsActive,
    bool IsCombo,
    bool UseOwnRecipeForCombo,
    int? ProductCategoryId,
    IReadOnlyCollection<ProductComboItemRequest>? ComboItems);

public record ProductComboItemDto(int Id, int ProductId, string ProductTitle, decimal Quantity);
public record ProductComboItemRequest(int ProductId, decimal Quantity);

public record IngredientCategoryDto(int Id, string Title, bool IsActive);
public record IngredientCategoryListRequest(string? Search = null, bool? IsActive = null);
public record IngredientCategoryRequest(string Title, bool IsActive = true);

public record IngredientDto(
    int Id,
    string Title,
    string Unit,
    decimal MinRemainderLimit,
    decimal AverageSelfPrice,
    bool IsActive,
    int? IngredientCategoryId,
    string? IngredientCategoryTitle,
    int? RecipeId,
    bool HasRecipe);

public record IngredientListRequest(string? Search = null, int? CategoryId = null, bool? IsActive = null, bool OnlyWithoutRecipe = false);
public record IngredientRequest(
    string Title,
    string Unit,
    decimal MinRemainderLimit,
    decimal AverageSelfPrice,
    bool IsActive,
    int? IngredientCategoryId);

public record RecipeDto(int Id, int? ProductId, int? IngredientId, decimal PortionYield, IReadOnlyCollection<RecipeItemDto> Items);
public record RecipeItemDto(int IngredientId, string IngredientTitle, decimal Quantity);
public record RecipeListRequest(string? Search = null, string? TargetType = null);
public record RecipeListItemDto(int Id, string TargetType, int TargetId, string TargetTitle, decimal PortionYield, int ItemsCount, decimal ItemsTotalQuantity);
public record RecipeListResponse(IReadOnlyCollection<RecipeListItemDto> Items, int ItemsCount);
public record UpsertRecipeRequest(int? ProductId, int? IngredientId, decimal PortionYield, IReadOnlyCollection<RecipeItemRequest> Items);
public record RecipeItemRequest(int IngredientId, decimal Quantity);

public record WarehouseDto(int Id, string Title, bool IsMain, bool IsActive);
public record WarehouseListRequest(string? Search = null, bool? IsActive = null);
public record WarehouseRequest(string Title, bool IsActive = true);

public record IngredientStockDto(
    int IngredientId,
    string IngredientTitle,
    string Unit,
    int WarehouseId,
    string WarehouseTitle,
    decimal Quantity,
    decimal AverageSelfPrice,
    decimal MinRemainderLimit,
    int? CategoryId,
    string? CategoryTitle)
{
    public decimal Total => Quantity * AverageSelfPrice;
    public bool IsBelowMinimum => Quantity <= MinRemainderLimit;
}

public record StockBalanceRequest(int? WarehouseId = null, int? CategoryId = null, string? Ingredient = null, bool OnlyOutOfStock = false);
public record StockBalanceResponse(IReadOnlyCollection<IngredientStockDto> Items, int ItemsCount, decimal Total, decimal TotalWithoutNegativeRemainder);

public record IncomeDto(int Id, int WarehouseId, string WarehouseTitle, DateTime CreatedAt, DateTime AcceptedAt, decimal TotalAmount, string? Comment, IReadOnlyCollection<IncomeItemDto> Items);
public record IncomeItemDto(int IngredientId, string IngredientTitle, decimal Quantity, decimal Price, decimal Total);
public record IncomeListRequest(DateTime? FromDate = null, DateTime? ToDate = null, int? WarehouseId = null, string? Search = null);
public record IncomeListResponse(IReadOnlyCollection<IncomeDto> Items, int ItemsCount, decimal Total);
public record CreateIncomeRequest(int WarehouseId, string? Comment, IReadOnlyCollection<CreateIncomeItemRequest> Items, DateTime? IncomedAt = null);
public record CreateIncomeItemRequest(int IngredientId, decimal Quantity, decimal Price);

public record PaymentMethodDto(int Id, string Title, bool IsActive);
public record PaymentMethodListRequest(string? Search = null, bool? IsActive = null);
public record PaymentMethodRequest(string Title, bool IsActive = true);

public record CustomerDto(int Id, string PhoneNumber, string FullName, string? Location);
public record CustomerListRequest(string? Search = null);
public record CustomerRequest(string PhoneNumber, string FullName, string? Location);

public record WorkerDto(int Id, string FullName, string? PhoneNumber, bool IsActive);
public record WorkerListRequest(string? Search = null, bool? IsActive = null);
public record WorkerRequest(string FullName, string? PhoneNumber, bool IsActive = true);

public record OrderDto(
    int Id,
    string OrderNumber,
    DateTime CreatedAt,
    decimal TotalAmount,
    int CustomerId,
    string CustomerPhoneNumber,
    string CustomerFullName,
    string? CustomerLocation,
    int WorkerId,
    string WorkerFullName,
    int WarehouseId,
    string WarehouseTitle,
    IReadOnlyCollection<OrderItemDto> Items,
    IReadOnlyCollection<OrderPaymentDto> Payments);

public record OrderItemDto(int ProductId, string ProductTitle, decimal Quantity, decimal Price, decimal Total, bool IsCombo);
public record OrderPaymentDto(int PaymentMethodId, string PaymentMethodTitle, decimal Amount);
public record OrderListRequest(DateTime? FromDate = null, DateTime? ToDate = null, string? Search = null, int? WorkerId = null, int? WarehouseId = null);
public record OrderListResponse(IReadOnlyCollection<OrderDto> Items, int ItemsCount, decimal TotalAmount);

public record CreateOrderRequest(
    int WarehouseId,
    int WorkerId,
    int PaymentMethodId,
    string CustomerPhoneNumber,
    string CustomerFullName,
    string? CustomerLocation,
    string? Comment,
    IReadOnlyCollection<CreateOrderItemRequest> Items);

public record CreateOrderItemRequest(int ProductId, decimal Quantity);
