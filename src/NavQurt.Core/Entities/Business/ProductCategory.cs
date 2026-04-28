namespace NavQurt.Core.Entities.Business;

public class ProductCategory : IEntity<int>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public int Priority { get; set; }

    public ProductCategory? ParentCategory { get; set; }
    public ICollection<ProductCategory> Children { get; set; } = new List<ProductCategory>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
