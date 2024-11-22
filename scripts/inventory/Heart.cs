using ColdMint.scripts.character;
using ColdMint.scripts.heal;
using ColdMint.scripts.pickable;
using Godot;

namespace ColdMint.scripts.inventory;

public partial class Heart : NonPickupItem
{

	/// <summary>
	/// <para>heal Amount</para>
	/// <para>恢复量</para>
	/// </summary>
	[Export]
	private int _healAmount;

	protected override void OnTouchPlayer(Player player)
	{
		base.OnTouchPlayer(player);
		var heal = new Heal
		{
			HealAmount = _healAmount,
			Source = this,
			MoveLeft = player.FacingLeft
		};
		//When the player touches the heart, the heart must be destroyed, regardless of whether the health is successfully restored.
		//无论是否成功恢复了健康值，在玩家触碰心时，都要销毁心。
		player.Heal(heal);
		QueueFree();
	}
}
