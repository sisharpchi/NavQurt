using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NavQurt.Shared.Requests;

public class PhoneNumberAuthorizationRequest
{
    [JsonPropertyName("phone_number")]
    [Required]
    public string PhoneNumber { get; set; } = default!;
}
