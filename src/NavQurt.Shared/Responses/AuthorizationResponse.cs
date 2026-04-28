using System.Text.Json.Serialization;

namespace NavQurt.Shared.Responses;

public class AuthorizationResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = default!;
}
