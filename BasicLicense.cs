using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LicenseMe;

/// <summary>
/// Basic License information given by the Git-Hub API
/// </summary>
public class BasicLicense
{
    public string Key { get; set; }
    public string Name { get; set; }
    [JsonPropertyName("spdx_id")]
    public string SpdxId { get; set; }
    public string Url { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }

    /// <summary>
    /// Asynchronously gets advanced information about this license from the GitHub-API
    /// </summary>
    /// <returns></returns>
    public async Task<AdvancedLicense?> GetAdvancedLicenseInformation()
    {
        return await GithubApiCommunicator.GetAdvancedInformation(this);
    }
}