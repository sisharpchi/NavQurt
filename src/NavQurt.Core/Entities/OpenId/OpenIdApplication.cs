using OpenIddict.EntityFrameworkCore.Models;

namespace NavQurt.Core.Entities;

public class OpenIdApplication : OpenIddictEntityFrameworkCoreApplication<long, OpenIdAuthorization, OpenIdToken>
{
    public string? CustomClientTag { get; set; }
}
