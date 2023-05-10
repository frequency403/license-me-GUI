using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LicenseMe;

public class BasicLicense
{
    public string Key { get; set; }
    public string Name { get; set; }
    [JsonPropertyName("spdx_id")]
    public string SpdxId { get; set; }
    public string Url { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }

    public async Task<AdvancedLicense?> GetAdvancedLicenseInformation()
    {
        return await GithubApiCommunicator.GetAdvancedInformation(this);
    }
}