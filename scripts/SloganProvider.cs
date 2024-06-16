using ColdMint.scripts.utils;

using Godot;

namespace ColdMint.scripts;

/// <summary>
/// <para>SloganProvider</para>
/// <para>标语提供器</para>
/// </summary>
public static class SloganProvider
{
    static SloganProvider()
    {
        // Calculate SloganCount From translation file
        // 从翻译文件中计算口号计数
        var sloganTrans = ResourceLoader.Load<OptimizedTranslation>("res://locals/Slogan.en.translation")!;
        SloganCount = sloganTrans.GetTranslatedMessageList().Length;
    }

    private static int SloganCount { get; }


    /// <summary>
    /// <para>Swipe the machine to get a slogan</para>
    /// <para>刷机获取一个标语</para>
    /// </summary>
    /// <returns></returns>
    public static string? GetSlogan()
    {
        var index = GD.Randi() % SloganCount;
        return TranslationServerUtils.Translate($"slogan_{index}");
    }
}