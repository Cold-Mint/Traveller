using Godot;

namespace ColdMint.scripts;

/// <summary>
/// <para>AssetHolder</para>
/// <para>资产持有者</para>
/// </summary>
public static class AssetHolder
{
    public static Texture2D? White25;
    public static Texture2D? White100;

    /// <summary>
    /// <para>Loading the game's static assets</para>
    /// <para>加载游戏的静态资产</para>
    /// </summary>
    public static void LoadStaticAsset()
    {
        White25 = ResourceLoader.Load<Texture2D>("res://sprites/light/White_25.png");
        White100 = ResourceLoader.Load<Texture2D>("res://sprites/light/White_100.png");
    }
}