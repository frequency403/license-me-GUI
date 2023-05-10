using System.Text.Json.Serialization;

namespace LicenseMe;

public class GitHubRateLimit
{
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
    [JsonPropertyName("remaining")]
    public int Remaining { get; set; }
    [JsonPropertyName("reset")]
    public long Reset { get; set; }
    [JsonPropertyName("used")]
    public int Used { get; set; }
    [JsonPropertyName("resource")]
    public string Resource { get; set; }
}