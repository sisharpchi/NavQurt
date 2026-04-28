using Microsoft.Extensions.DependencyInjection;
using NavQurt.Application.Common;
using NavQurt.Application.Contracts;
using NavQurt.Application.Services;

namespace NavQurt.Application;

public static class Registrar
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IProductCategoryService, ProductCategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IIngredientCategoryService, IngredientCategoryService>();
        services.AddScoped<IIngredientService, IngredientService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<IIncomeService, IncomeService>();
        services.AddScoped<IPaymentMethodService, PaymentMethodService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IWorkerService, WorkerService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<StockMutationService>();
        services.AddScoped<RecipeCalculator>();

        return services;
    }
}
