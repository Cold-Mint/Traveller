using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts;

/// <summary>
/// <para>SloganProvider</para>
/// <para>标语提供器</para>
/// </summary>
public static class SloganProvider
{
    private const int MaxSloganIndex = 5;


    /// <summary>
    /// <para>Swipe the machine to get a slogan</para>
    /// <para>刷机获取一个标语</para>
    /// </summary>
    /// <returns></returns>
    public static string? GetSlogan()
    {
        var index = GD.Randi() % MaxSloganIndex + 1;
        return TranslationServerUtils.Translate("slogan_" + index);
    }
}