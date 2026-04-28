using OpenIddict.Abstractions;

namespace NavQurt.Server.Services;

public class TokenRevocationService
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictTokenManager _tokenManager;

    public TokenRevocationService(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictTokenManager tokenManager)
    {
        _applicationManager = applicationManager;
        _tokenManager = tokenManager;
    }

    public async Task RevokeClientTokensAsync(string clientId)
    {
        var application = await _applicationManager.FindByClientIdAsync(clientId);
        if (application == null)
        {
            return;
        }

        var applicationId = await _applicationManager.GetIdAsync(application);
        if (applicationId == null)
        {
            return;
        }

        await foreach (var token in _tokenManager.FindByApplicationIdAsync(applicationId))
        {
            var status = await _tokenManager.GetStatusAsync(token);
            if (status == OpenIddictConstants.Statuses.Valid)
            {
                await _tokenManager.TryRevokeAsync(token);
            }
        }
    }
}
