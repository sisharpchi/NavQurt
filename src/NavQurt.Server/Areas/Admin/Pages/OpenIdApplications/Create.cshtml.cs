using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace NavQurt.Server.Areas.Admin.Pages.OpenIdApplications;

public class CreateModel : PageModel
{
    private readonly IOpenIddictApplicationManager _applicationManager;

    public CreateModel(IOpenIddictApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? CreatedSecret { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (await _applicationManager.FindByClientIdAsync(Input.ClientId) is not null)
        {
            ModelState.AddModelError(nameof(Input.ClientId), "Client ID already exists.");
            return Page();
        }

        var isConfidential = !string.IsNullOrWhiteSpace(Input.ClientSecret);
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = Input.ClientId,
            DisplayName = Input.DisplayName
        };

        descriptor.Permissions.Add(Permissions.Endpoints.Token);
        descriptor.Permissions.Add(Permissions.Prefixes.Scope + "read");
        descriptor.Permissions.Add(Permissions.Prefixes.Scope + "write");

        if (Input.AllowPasswordFlow)
        {
            descriptor.Permissions.Add(Permissions.GrantTypes.Password);
            descriptor.Permissions.Add(Permissions.GrantTypes.RefreshToken);
            descriptor.Permissions.Add(Permissions.Prefixes.Scope + Scopes.OfflineAccess);
        }

        if (Input.AllowClientCredentialsFlow)
        {
            descriptor.Permissions.Add(Permissions.GrantTypes.ClientCredentials);
        }

        if (isConfidential)
        {
            await _applicationManager.CreateAsync(descriptor, Input.ClientSecret);
        }
        else
        {
            await _applicationManager.CreateAsync(descriptor);
        }
        CreatedSecret = Input.ClientSecret;
        ModelState.Clear();
        Input = new InputModel();

        return Page();
    }

    public class InputModel
    {
        [Required]
        [Display(Name = "Client ID")]
        public string ClientId { get; set; } = string.Empty;

        [Display(Name = "Display name")]
        public string? DisplayName { get; set; }

        [Display(Name = "Client secret")]
        public string? ClientSecret { get; set; }

        [Display(Name = "Allow password flow")]
        public bool AllowPasswordFlow { get; set; } = true;

        [Display(Name = "Allow client credentials flow")]
        public bool AllowClientCredentialsFlow { get; set; }
    }
}
