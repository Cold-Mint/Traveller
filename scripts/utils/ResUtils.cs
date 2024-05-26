using System;

namespace ColdMint.scripts.utils;

public class ResUtils
{
    /// <summary>
    /// <para>The game returns the res directory with a.remap suffix at runtime, causing an error while loading the resource</para>
    /// <para>游戏在运行时返回res目录后带有.remap后缀，导致加载资源时出错</para>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetAbsolutePath(string path)
    {
        var index = path.LastIndexOf(".remap", StringComparison.Ordinal);
        return index > -1 ? path[..index] : path;
    }
}