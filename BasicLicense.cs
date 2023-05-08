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
        using var client = new HttpClient();
        var response = new HttpResponseMessage();
        client.DefaultRequestHeaders.Add("User-Agent", "frequency403");
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        response = await client.GetAsync(Url);
        return JsonSerializer.Deserialize<AdvancedLicense>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}