using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages;

public class IndexModel(
    IProductService productService,
    IIngredientService ingredientService,
    IOrderService orderService) : PageModel
{
    public int ProductCount { get; private set; }
    public int IngredientCount { get; private set; }
    public int TodayOrders { get; private set; }
    public decimal TodaySales { get; private set; }
    public IReadOnlyCollection<OrderDto> RecentOrders { get; private set; } = Array.Empty<OrderDto>();

    public async Task OnGet(CancellationToken cancellationToken)
    {
        var products = await productService.GetListAsync(cancellationToken);
        var ingredients = await ingredientService.GetListAsync(cancellationToken);
        var orders = await orderService.GetListAsync(cancellationToken);

        ProductCount = products.Value?.Count ?? 0;
        IngredientCount = ingredients.Value?.Count ?? 0;

        var today = DateTime.UtcNow.Date;
        var orderList = orders.Value ?? Array.Empty<OrderDto>();
        TodayOrders = orderList.Count(x => x.CreatedAt.Date == today);
        TodaySales = orderList.Where(x => x.CreatedAt.Date == today).Sum(x => x.TotalAmount);
        RecentOrders = orderList.Take(8).ToList();
    }
}
