using System.Collections.Generic;

namespace LicenseMe;

public static class LicenseHolder
{
    public static IAsyncEnumerable<BasicLicense?>? Licenses { get; set; } = null;

    public static void InitLicenses()
    {
        Licenses = GithubAPICommunicator.GetLicenses();
    }

    public static void DisposeLicenses()
    {
        Licenses = null;
    }
}