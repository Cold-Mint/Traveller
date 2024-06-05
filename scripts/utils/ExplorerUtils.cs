using System;
using System.Diagnostics;

namespace ColdMint.scripts.utils;

/// <summary>
/// <para>Explorer Utils</para>
/// <para>资源管理器工具</para>
/// </summary>
public static class ExplorerUtils
{
    /// <summary>
    /// <para>Call Explorer to open the directory</para>
    /// <para>调用资源管理器打开目录</para>
    /// </summary>
    /// <param name="path">
    ///<para>要打开的目录路径</para>
    ///<para>The path of the directory to open</para>
    /// </param>
    public static void OpenFolder(string path)
    {
        var osEnum = Config.GetOs();
        switch (osEnum)
        {
            case Config.OsEnum.Windows:
                var startInfoWindows = new ProcessStartInfo
                {
                    Arguments = path,
                    FileName = "explorer.exe"
                };
                Process.Start(startInfoWindows);
                break;
            case Config.OsEnum.Linux:
                // Use the xdg-open command to open the directory on Linux
                // 使用xdg-open命令在Linux上打开目录
                var startInfoLinux = new ProcessStartInfo
                {
                    Arguments = path,
                    FileName = "xdg-open"
                };
                Process.Start(startInfoLinux);
                break;
            case Config.OsEnum.Android:
                // A different approach may be required on Android, as there is usually no desktop environment
                // A general Intent is used here to open the file manager, but a specific implementation may be required
                // The following code is only an indication, the actual implementation may need to be adjusted according to the Android API
                // Android上可能需要使用不同的方法，因为通常没有桌面环境
                // 这里使用一个通用的Intent来打开文件管理器，但可能需要具体的实现
                // 以下代码只是一个示意，实际的实现可能需要根据Android API进行调整
                var startInfoAndroid = new ProcessStartInfo
                {
                    Arguments = "VIEW",
                    FileName = "content://com.android.externalstorage.documents/tree/primary%3ADocuments"
                };
                Process.Start(startInfoAndroid);
                break;
            case Config.OsEnum.Unknown:
            case Config.OsEnum.Macos:
            case Config.OsEnum.Ios:
            case Config.OsEnum.Web:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// <para>Whether opening directories using Explorer is supported on the current system</para>
    /// <para>在当前系统上是否支持使用资源管理器打开目录</para>
    /// </summary>
    /// <returns></returns>
    public static bool SupportOpenDirectory()
    {
        var osEnum = Config.GetOs();
        return osEnum switch
        {
            Config.OsEnum.Windows or Config.OsEnum.Linux or Config.OsEnum.Android => true,
            _ => false
        };
    }
}