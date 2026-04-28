using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NavQurt.Core.Entities;
using NavQurt.Server.Services;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace NavQurt.Server.Controllers;

[Route("/security/oauth")]
[Route("/api/uzum/security/oauth")]
[ApiController]
public class OAuthAuthorizationController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IOpenIddictApplicationManager _applicationManager;

    public OAuthAuthorizationController(
        UserManager<User> userManager,
        IOpenIddictApplicationManager applicationManager)
    {
        _userManager = userManager;
        _applicationManager = applicationManager;
    }

    [HttpPost("token")]
    [IgnoreAntiforgeryToken]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new ArgumentException("OpenIddict request is not available.");

        if (request.IsClientCredentialsGrantType())
        {
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId!);
            if (application == null)
            {
                return Unauthorized(new OpenIddictResponse
                {
                    Error = Errors.InvalidClient,
                    ErrorDescription = "The application details cannot be found."
                });
            }

            var identity = new ClaimsIdentity(
                TokenValidationParameters.DefaultAuthenticationType,
                Claims.Name,
                Claims.Role);

            identity.SetClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(application));
            identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));
            identity.SetScopes(request.GetScopes());
            identity.SetDestinations(GetDestinations);

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsPasswordGrantType())
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Unauthorized(new OpenIddictResponse
                {
                    Error = Errors.InvalidRequest,
                    ErrorDescription = "Username and password are required."
                });
            }

            var user = await _userManager.FindByNameAsync(request.Username)
                ?? _userManager.Users.FirstOrDefault(u => u.PhoneNumber == request.Username);

            if (user == null)
            {
                return Unauthorized(new OpenIddictResponse
                {
                    Error = Errors.InvalidGrant,
                    ErrorDescription = "User was not found."
                });
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password)
                || user.Code == request.Password;

            if (!isPasswordValid)
            {
                return Unauthorized(new OpenIddictResponse
                {
                    Error = Errors.InvalidGrant,
                    ErrorDescription = "Invalid credentials."
                });
            }

            var principal = await CreatePrincipalAsync(user, request.GetScopes());
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsRefreshTokenGrantType())
        {
            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var principal = result.Principal;

            if (principal?.Identity?.IsAuthenticated != true)
            {
                return Unauthorized(new OpenIddictResponse
                {
                    Error = Errors.InvalidGrant,
                    ErrorDescription = "The refresh token is invalid."
                });
            }

            var userId = principal.GetClaim(Claims.Subject);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            var refreshedPrincipal = await CreatePrincipalAsync(user, principal.GetScopes());
            return SignIn(refreshedPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        return BadRequest(new OpenIddictResponse
        {
            Error = Errors.UnsupportedGrantType,
            ErrorDescription = "The specified grant type is not implemented."
        });
    }

    [HttpPost("revoke-client")]
    public async Task<IActionResult> RevokeClient([FromServices] TokenRevocationService tokenRevocationService, string clientId)
    {
        await tokenRevocationService.RevokeClientTokensAsync(clientId);
        return Ok($"Tokens for {clientId} were revoked.");
    }

    private async Task<ClaimsPrincipal> CreatePrincipalAsync(User user, IEnumerable<string> scopes)
    {
        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            Claims.Name,
            Claims.Role);

        identity.SetClaim(Claims.JwtId, Guid.NewGuid().ToString());
        identity.SetClaim(Claims.Subject, user.Id);
        identity.SetClaim(ClaimTypes.NameIdentifier, user.Id);
        identity.SetClaim(Claims.Name, user.UserName);
        identity.SetClaim(Claims.GivenName, user.FirstName ?? string.Empty);
        identity.SetClaim(Claims.FamilyName, user.LastName ?? string.Empty);
        identity.SetClaim(Claims.PhoneNumber, user.PhoneNumber ?? string.Empty);

        foreach (var role in await _userManager.GetRolesAsync(user))
        {
            identity.AddClaim(new Claim(Claims.Role, role));
        }

        var requestedScopes = new HashSet<string>(scopes);
        requestedScopes.Add(Scopes.OfflineAccess);
        identity.SetScopes(requestedScopes);
        identity.SetDestinations(GetDestinations);

        return new ClaimsPrincipal(identity);
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        return claim.Type switch
        {
            Claims.Name or Claims.Subject or Claims.Role => [Destinations.AccessToken, Destinations.IdentityToken],
            _ => [Destinations.AccessToken],
        };
    }
}
