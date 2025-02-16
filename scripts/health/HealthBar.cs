using Godot;

namespace ColdMint.scripts.health;

/// <summary>
/// <para>Health bar</para>
/// <para>健康条</para>
/// </summary>
public partial class HealthBar : TextureProgressBar
{
    /// <summary>
    /// <para>Set to a player-friendly blood bar style</para>
    /// <para>设置为与玩家友好的血条样式</para>
    /// </summary>
    public void SetFriendlyTones()
    {
        //The border color is Open Gray 0
        //边框颜色为Open Gray 0
        TintOver = Config.ColorConfig.BorderColor;
        TintUnder = Config.ColorConfig.FriendlyBackgroundColor;
        TintProgress = Config.ColorConfig.FriendlyColor;
    }

    /// <summary>
    /// <para>Set the blood bar style to be hostile to the player</para>
    /// <para>设置为与玩家敌对的血条样式</para>
    /// </summary>
    public void SetEnemyTones()
    {
        TintOver = Config.ColorConfig.BorderColor;
        TintUnder = Config.ColorConfig.EnemyBackgroundColor;
        TintProgress = Config.ColorConfig.EnemyColor;
    }
}