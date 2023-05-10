﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace LicenseMe;

public static class GithubApiCommunicator
{
    private static GitHubRateLimit? RateLimit { get; }

    private static HttpClient? _httpClient;


    private static void PrepareClient()
    {
        _httpClient?.DefaultRequestHeaders.Clear();
        _httpClient ??= new();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", Settings.SettingValues.GitHubUser);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        if (Settings.SettingValues is not null && Settings.SettingValues.GitHubToken is not null)
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Settings.SettingValues.GitHubToken);
        _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
    }

    static GithubApiCommunicator()
    {
        PrepareClient();
        try
        {
            RateLimit = JsonSerializer.Deserialize<GitHubRateLimit>(
                _httpClient.GetAsync("https://api.github.com/rate_limit").Result.Content.ReadAsStringAsync().Result
                    .Split("rate")[1].Replace("}}", "}").Replace("\":{", "{").Trim());
        }
        catch (Exception)
        {
            RateLimit = null;
        }
    }

    public static async IAsyncEnumerable<BasicLicense?> GetLicenses(
        string url = "https://api.github.com/licenses?per_page=100")
    {
        PrepareClient();
        var response = await _httpClient.GetAsync(url);
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

    public static async Task<AdvancedLicense?> GetAdvancedInformation(BasicLicense license)
    {
        PrepareClient();
        var response = await _httpClient.GetAsync(license.Url);
        return JsonSerializer.Deserialize<AdvancedLicense>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public static bool? RateLimitReached(out DateTime? expired)
    {
        if (RateLimit is null)
        {
            expired = null;
            return null;
        }

        expired = DateTimeOffset.FromUnixTimeSeconds(RateLimit.Reset).UtcDateTime.ToLocalTime();
        return RateLimit.Limit == RateLimit.Used;
    }
}