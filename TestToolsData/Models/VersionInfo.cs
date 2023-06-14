using Microsoft.Extensions.Configuration;

namespace TestToolsData.Models;

public sealed class VersionInfo
{
    public string? AppSettingsVersion { get; set; }


    public static VersionInfo? Create(IConfiguration configuration)
    {
        var versionInfoSettings = configuration.GetSection("VersionInfo");
        var versionInfo = versionInfoSettings.Get<VersionInfo>();
        return versionInfo;
    }
}