using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace LicenseMe;

public static class GithubAPICommunicator
{
    public static async IAsyncEnumerable<BasicLicense?> GetLicenses(
        string url = "https://api.github.com/licenses?per_page=100")
    {
        using var client = new HttpClient();
        var response = new HttpResponseMessage();
        client.DefaultRequestHeaders.Add("User-Agent", "frequency403");
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        if(Settings.SettingValues is not null && Settings.SettingValues.GitHubToken is not null) client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Settings.SettingValues.GitHubToken}");
        client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        response = await client.GetAsync(url);
        var formatted = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            switch (response.ReasonPhrase)
            {
                case { } x when x.Contains("limit exceeded"):
                    MessageBox.Show("API Limit exceeded!");
                    break;
                default:
                    MessageBox.Show("API could not been contacted. Do you have a Internet Connection?");
                    break;
            }

            Application.Current.Shutdown(1);
        }

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