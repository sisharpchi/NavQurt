using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NavQurt.Shared.Requests;

public class AuthorizationRequest
{
    [JsonPropertyName("phone_number")]
    [Required]
    public string PhoneNumber { get; set; } = default!;

    [JsonPropertyName("code")]
    [Required]
    public string Code { get; set; } = default!;
}
