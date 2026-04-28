using Microsoft.AspNetCore.Identity;

namespace NavQurt.Core.Entities;

public class User : IdentityUser, IEntity<string>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Token { get; set; }
    public string? Code { get; set; }

    public string FullName => string.Join(" ", new[] { LastName, FirstName }.Where(x => !string.IsNullOrWhiteSpace(x)));
}
