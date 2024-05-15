using System.Collections.Generic;
using Godot;
using FileAccess = Godot.FileAccess;

namespace ColdMint.scripts;

/// <summary>
/// <para>SloganProvider</para>
/// <para>标语提供器</para>
/// </summary>
public static class SloganProvider
{
    private static string _csvPath = "res://locals/slogan.csv";

    private static string[]? _sloganKeys;

    /// <summary>
    /// <para>Loading CSV file</para>
    /// <para>加载CSV文件</para>
    /// </summary>
    public static void LoadSloganCsv()
    {
        var exists = FileAccess.FileExists(_csvPath);
        if (!exists)
        {
            return;
        }

        using var file = FileAccess.Open(_csvPath, FileAccess.ModeFlags.Read);
        var content = file.GetAsText();
        var lineStrings = content.Split('\n');
        var keys = new List<string>();
        foreach (var lineString in lineStrings)
        {
            var index = lineString.IndexOf(',');
            if (index > -1)
            {
                keys.Add(lineString[..index]);
            }
        }

        if (keys.Count > 0)
        {
            keys.RemoveAt(0);
        }

        _sloganKeys = keys.ToArray();
    }

    /// <summary>
    /// <para>Swipe the machine to get a slogan</para>
    /// <para>刷机获取一个标语</para>
    /// </summary>
    /// <returns></returns>
    public static string? GetSlogan()
    {
        if (_sloganKeys == null || _sloganKeys.Length == 0)
        {
            return null;
        }

        return TranslationServer.Translate(_sloganKeys[GD.Randi() % _sloganKeys.Length]);
    }
}