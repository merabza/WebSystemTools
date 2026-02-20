using Microsoft.Extensions.Configuration;

namespace WebSystemTools.TestToolsData.Models;

public sealed class VersionInfo
{
    public string? AppSettingsVersion { get; set; }

    public static VersionInfo? Create(IConfiguration configuration)
    {
        IConfigurationSection versionInfoSettings = configuration.GetSection("VersionInfo");
        var versionInfo = versionInfoSettings.Get<VersionInfo>();
        return versionInfo;
    }
}
