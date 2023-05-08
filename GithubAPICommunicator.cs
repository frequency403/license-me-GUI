using System;
using System.Collections.Generic;
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

public class AdvancedLicense
{
    public string Key { get; set; }
    public string Name { get; set; }
    [JsonPropertyName("spdx_id")]
    public string SpdxId { get; set; }
    public string Url { get; set; }
    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }
    public string Description { get; set; }
    public string Implementation { get; set; }
    public IEnumerable<string> Permissions { get; set; }
    public IEnumerable<string> Conditions { get; set; }
    public IEnumerable<string> Limitations { get; set; }
    public string Body { get; set; }
    public bool Featured { get; set; }

    public void Personalize(string fullUsername)
    {
        Body = Body.Replace("[year]", DateTime.Now.Year.ToString()).Replace("[fullname]", fullUsername);
    }
}

public static class GithubAPICommunicator
{
    public static async IAsyncEnumerable<BasicLicense?> GetLicenses(string url = "https://api.github.com/licenses?per_page=100")
    {
        using var client = new HttpClient();
        var response = new HttpResponseMessage();
        client.DefaultRequestHeaders.Add("User-Agent", "frequency403");
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        response = await client.GetAsync(url);
        var formatted = await response.Content.ReadAsStringAsync();
        
        foreach (var json in formatted.Replace("[", "").Replace("]", "").Replace("},{", "}|||{").Split("|||"))
        {
            yield return JsonSerializer.Deserialize<BasicLicense>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}