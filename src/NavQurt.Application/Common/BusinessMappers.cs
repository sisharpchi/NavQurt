using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;

namespace NavQurt.Application.Common;

internal static class BusinessMappers
{
    public static ProductCategoryDto ToDto(this ProductCategory entity) =>
        new(entity.Id, entity.Title, entity.ParentCategoryId, entity.IsActive, entity.Priority);

    public static ProductDto ToDto(this Product entity) =>
        new(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.Price,
            entity.SelfPrice,
            entity.IsActive,
            entity.IsCombo,
            entity.UseOwnRecipeForCombo,
            entity.ProductCategoryId,
            entity.ProductCategory?.Title,
            entity.Recipe is { IsDeleted: false } ? entity.Recipe.Id : null,
            entity.Recipe is { IsDeleted: false },
            entity.ComboItems.Select(x => new ProductComboItemDto(x.Id, x.ProductId, x.Product.Title, x.Quantity)).ToList());

    public static IngredientCategoryDto ToDto(this IngredientCategory entity) =>
        new(entity.Id, entity.Title, entity.IsActive);

    public static IngredientDto ToDto(this Ingredient entity) =>
        new(
            entity.Id,
            entity.Title,
            entity.Unit,
            entity.MinRemainderLimit,
            entity.AverageSelfPrice,
            entity.IsActive,
            entity.IngredientCategoryId,
            entity.IngredientCategory?.Title,
            entity.Recipe is { IsDeleted: false } ? entity.Recipe.Id : null,
            entity.Recipe is { IsDeleted: false });

    public static RecipeDto ToDto(this Recipe entity) =>
        new(
            entity.Id,
            entity.ProductId,
            entity.IngredientId,
            entity.PortionYield,
            entity.Items.Select(x => new RecipeItemDto(x.IngredientId, x.Ingredient.Title, x.Quantity)).ToList());

    public static RecipeListItemDto ToListDto(this Recipe entity)
    {
        var isProductRecipe = entity.ProductId.HasValue;
        var targetId = entity.ProductId ?? entity.IngredientId ?? 0;
        var targetTitle = isProductRecipe
            ? entity.Product?.Title ?? $"Product #{targetId}"
            : entity.Ingredient?.Title ?? $"Ingredient #{targetId}";

        return new RecipeListItemDto(
            entity.Id,
            isProductRecipe ? "Product" : "Ingredient",
            targetId,
            targetTitle,
            entity.PortionYield,
            entity.Items.Count,
            entity.Items.Sum(x => x.Quantity));
    }

    public static WarehouseDto ToDto(this Warehouse entity) =>
        new(entity.Id, entity.Title, entity.IsMain, entity.IsActive);

    public static IncomeDto ToDto(this Income entity) =>
        new(
            entity.Id,
            entity.WarehouseId,
            entity.Warehouse?.Title ?? string.Empty,
            entity.CreatedAt,
            entity.AcceptedAt,
            entity.TotalAmount,
            entity.Comment,
            entity.Items.Select(x => new IncomeItemDto(x.IngredientId, x.Ingredient.Title, x.Quantity, x.Price, x.Quantity * x.Price)).ToList());

    public static PaymentMethodDto ToDto(this PaymentMethod entity) =>
        new(entity.Id, entity.Title, entity.IsActive);

    public static CustomerDto ToDto(this Customer entity) =>
        new(entity.Id, entity.PhoneNumber, entity.FullName, entity.Location);

    public static WorkerDto ToDto(this Worker entity) =>
        new(entity.Id, entity.FullName, entity.PhoneNumber, entity.IsActive);

    public static OrderDto ToDto(this Order entity) =>
        new(
            entity.Id,
            entity.OrderNumber,
            entity.CreatedAt,
            entity.TotalAmount,
            entity.CustomerId,
            entity.CustomerPhoneNumber,
            entity.CustomerFullName,
            entity.CustomerLocation,
            entity.WorkerId,
            entity.Worker?.FullName ?? string.Empty,
            entity.WarehouseId,
            entity.Warehouse?.Title ?? string.Empty,
            entity.Items.Select(x => new OrderItemDto(x.ProductId, x.ProductTitle, x.Quantity, x.Price, x.Quantity * x.Price, x.IsCombo)).ToList(),
            entity.Payments.Select(x => new OrderPaymentDto(x.PaymentMethodId, x.PaymentMethod.Title, x.Amount)).ToList());
}
