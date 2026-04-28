namespace NavQurt.Server.Options;

public class OpenIddictSeedOptions
{
    public const string Key = "OpenIddictSeed";

    public string WebClientId { get; set; } = "web-client";
    public string WebClientDisplayName { get; set; } = "NavQurt Web UI";
    public string ServiceClientId { get; set; } = "service-client";
    public string ServiceClientSecret { get; set; } = "secret";
    public string ServiceClientDisplayName { get; set; } = "NavQurt Service Client";
}
