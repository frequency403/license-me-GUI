using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LicenseMe;

public class AdvancedLicense : BasicLicense
{
    // public string Key { get; set; }
    // public string Name { get; set; }
    // [JsonPropertyName("spdx_id")]
    // public string SpdxId { get; set; }
    // public string Url { get; set; }
    // [JsonPropertyName("node_id")]
    // public string NodeId { get; set; }
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