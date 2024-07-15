using ColdMint.scripts.debug;
using ColdMint.scripts.openObserve;
using ColdMint.scripts.serialization;
using Godot;

namespace ColdMint.scripts;

public class AppConfig
{
    /// <summary>
    /// <para>Load configuration from file</para>
    /// <para>从文件加载配置</para>
    /// </summary>
    public static AppConfigData? LoadFromFile()
    {
        var appConfigExists = FileAccess.FileExists(Config.AppConfigPath);
        if (!appConfigExists)
        {
            LogCat.LogWarning("appConfig_not_exist");
            return null;
        }

        var appConfigFileAccess = FileAccess.Open(Config.AppConfigPath, FileAccess.ModeFlags.Read);
        var yamlData = appConfigFileAccess.GetAsText();
        appConfigFileAccess.Close();
        return YamlSerialization.Deserialize<AppConfigData>(yamlData);
    }


    /// <summary>
    /// <para>ApplyAppConfig</para>
    /// <para>应用配置</para>
    /// </summary>
    /// <param name="appConfigData"></param>
    public static void ApplyAppConfig(AppConfigData appConfigData)
    {
        if (appConfigData.OpenObserve != null)
        {
            LogCollector.UpdateHttpClient(appConfigData.OpenObserve);
        }
    }
}

public class AppConfigData
{
    /// <summary>
    /// <para>OpenObserve configuration information</para>
    /// <para>OpenObserve的配置信息</para>
    /// </summary>
    public OpenObserve? OpenObserve { get; set; }
}

/// <summary>
/// <para>OpenObserve Configuration information</para>
/// <para>OpenObserve配置信息</para>
/// </summary>
public class OpenObserve
{
    /// <summary>
    /// <para>server address</para>
    /// <para>服务器地址</para>
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// <para>Access Token</para>
    /// <para>访问密匙</para>
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// <para>Organization ID</para>
    /// <para>组织ID</para>
    /// </summary>
    public string? OrgId { get; set; }
    
    /// <summary>
    /// <para>Stream Name</para>
    /// <para>流名称</para>
    /// </summary>
    public string? StreamName { get; set; }
}