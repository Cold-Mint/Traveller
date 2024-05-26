using System;

namespace ColdMint.scripts.utils;

/// <summary>
/// <para>ResUtils</para>
/// <para>资源文件工具类</para>
/// </summary>
public static class ResUtils
{
    /// <summary>
    /// <para>The suffix that the game engine adds to the resource file while the game is running</para>
    /// <para>游戏运行时，游戏引擎为资源文件添加的后缀</para>
    /// </summary>
    private const string Suffix = ".remap";

    /// <summary>
    /// <para>The game returns the res directory with a.remap suffix at runtime, causing an error while loading the resource</para>
    /// <para>游戏在运行时返回res目录后带有.remap后缀，导致加载资源时出错</para>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetEditorResPath(string path)
    {
        if (Config.IsEditor())
        {
            return path;
        }

        var index = path.LastIndexOf(Suffix, StringComparison.Ordinal);
        return index > -1 ? path[..index] : path;
    }

    /// <summary>
    /// <para>Gets the runtime path of the resource file</para>
    /// <para>获取资源文件运行时路径</para>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetRunTimeResPath(string path)
    {
        if (Config.IsEditor())
        {
            return path;
        }

        //Determine whether the path is a file path
        //判断路径是否为文件路径
        var symbolIndex = path.LastIndexOf('.');
        if (symbolIndex > -1)
        {
            //Found the file symbol
            //找到了文件符号
            var index = path.LastIndexOf(Suffix, StringComparison.Ordinal);
            return index > -1 ? path : path + Suffix;
        }
        else
        {
            return path;
        }
    }
}