namespace NavQurt.Core.Entities.Business;

public class Product : IEntity<int>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal SelfPrice { get; set; }
    public string? PhotoPath { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public bool IsCombo { get; set; }
    public bool UseOwnRecipeForCombo { get; set; }
    public int? ProductCategoryId { get; set; }

    public ProductCategory? ProductCategory { get; set; }
    public Recipe? Recipe { get; set; }
    public ICollection<ProductComboItem> ComboItems { get; set; } = new List<ProductComboItem>();
    public ICollection<ProductComboItem> IncludedInCombos { get; set; } = new List<ProductComboItem>();
}
