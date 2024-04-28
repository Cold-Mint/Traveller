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
		//边框颜色为Open Gray 0
		TintOver = new Color("#f8f9fa");
		//背景色为Open Green 1
		TintUnder = new Color("#d3f9d8");
		//进度条颜色为Open Green 5
		TintProgress = new Color("#51cf66");
	}
	
	/// <summary>
	/// <para>Set the blood bar style to be hostile to the player</para>
	/// <para>设置为与玩家敌对的血条样式</para>
	/// </summary>
	public void SetEnemyTones()
	{
		//边框颜色为Open Gray 0
		TintOver = new Color("#f8f9fa");
		//背景色为Open Red 1
		TintUnder = new Color("#ffe3e3");
		//进度条颜色为Open Red 5
		TintProgress = new Color("#ff6b6b");
	}
}
