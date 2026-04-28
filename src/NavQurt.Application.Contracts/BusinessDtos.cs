namespace NavQurt.Application.Contracts;

public record ProductCategoryDto(int Id, string Title, int? ParentCategoryId, bool IsActive, int Priority);
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
    IReadOnlyCollection<ProductComboItemDto> ComboItems);

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
public record IngredientCategoryRequest(string Title, bool IsActive = true);

public record IngredientDto(
    int Id,
    string Title,
    string Unit,
    decimal MinRemainderLimit,
    decimal AverageSelfPrice,
    bool IsActive,
    int? IngredientCategoryId);

public record IngredientRequest(
    string Title,
    string Unit,
    decimal MinRemainderLimit,
    decimal AverageSelfPrice,
    bool IsActive,
    int? IngredientCategoryId);

public record RecipeDto(int Id, int? ProductId, int? IngredientId, decimal PortionYield, IReadOnlyCollection<RecipeItemDto> Items);
public record RecipeItemDto(int IngredientId, string IngredientTitle, decimal Quantity);
public record UpsertRecipeRequest(int? ProductId, int? IngredientId, decimal PortionYield, IReadOnlyCollection<RecipeItemRequest> Items);
public record RecipeItemRequest(int IngredientId, decimal Quantity);

public record WarehouseDto(int Id, string Title, bool IsMain, bool IsActive);
public record WarehouseRequest(string Title, bool IsActive = true);

public record IngredientStockDto(int IngredientId, string IngredientTitle, string Unit, int WarehouseId, string WarehouseTitle, decimal Quantity);

public record IncomeDto(int Id, int WarehouseId, DateTime CreatedAt, DateTime AcceptedAt, decimal TotalAmount, string? Comment, IReadOnlyCollection<IncomeItemDto> Items);
public record IncomeItemDto(int IngredientId, string IngredientTitle, decimal Quantity, decimal Price, decimal Total);
public record CreateIncomeRequest(int WarehouseId, string? Comment, IReadOnlyCollection<CreateIncomeItemRequest> Items);
public record CreateIncomeItemRequest(int IngredientId, decimal Quantity, decimal Price);

public record PaymentMethodDto(int Id, string Title, bool IsActive);
public record PaymentMethodRequest(string Title, bool IsActive = true);

public record CustomerDto(int Id, string PhoneNumber, string FullName, string? Location);
public record CustomerRequest(string PhoneNumber, string FullName, string? Location);

public record WorkerDto(int Id, string FullName, string? PhoneNumber, bool IsActive);
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
    int WarehouseId,
    IReadOnlyCollection<OrderItemDto> Items,
    IReadOnlyCollection<OrderPaymentDto> Payments);

public record OrderItemDto(int ProductId, string ProductTitle, decimal Quantity, decimal Price, decimal Total, bool IsCombo);
public record OrderPaymentDto(int PaymentMethodId, string PaymentMethodTitle, decimal Amount);

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
