using OpenIddict.EntityFrameworkCore.Models;

namespace NavQurt.Core.Entities;

public class OpenIdAuthorization : OpenIddictEntityFrameworkCoreAuthorization<long, OpenIdApplication, OpenIdToken>
{
}
