using Microsoft.AspNetCore.Identity;

namespace NavQurt.Core.Entities;

public class AppRole : IdentityRole<string>, IEntity<string>
{
    public AppRole()
    {
    }

    public AppRole(string name)
        : base(name.StartsWith("app.", StringComparison.OrdinalIgnoreCase) ? name : $"app.{name}")
    {
        DisplayName = Name;
    }

    public override string Id { get; set; } = Guid.NewGuid().ToString();
    public string? DisplayName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
