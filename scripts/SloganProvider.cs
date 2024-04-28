using Godot;

namespace ColdMint.scripts;

/// <summary>
/// <para>SloganProvider</para>
/// <para>标语提供器</para>
/// </summary>
public static class SloganProvider
{
    /// <summary>
    /// <para>Define how many banners you want to display</para>
    /// <para>定义共有多少条标语需要展示</para>
    /// </summary>
    private const int total = 12;

    /// <summary>
    /// <para>Swipe the machine to get a slogan</para>
    /// <para>刷机获取一个标语</para>
    /// </summary>
    /// <returns></returns>
    public static string GetSlogan()
    {
        return TranslationServer.Translate("slogan_" + GD.RandRange(1, total));
    }
}