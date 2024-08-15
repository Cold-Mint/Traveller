using Godot;

namespace ColdMint.scripts.damage;

/// <summary>
/// <para>DamageNumber</para>
/// <para>伤害数字</para>
/// </summary>
public partial class DamageNumber : CharacterBody2D
{
	private VisibleOnScreenNotifier2D? _visibleOnScreenNotifier2D;

	public override void _Ready()
	{
		_visibleOnScreenNotifier2D = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		_visibleOnScreenNotifier2D.ScreenExited += ScreenExited;
	}

	private void ScreenExited()
	{
		//When the damage number leaves the screen, destroy the damage number
		//当伤害数字离开屏幕时，销毁伤害数字
		QueueFree();
	}

	private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private bool _enableGravity;

	public new void SetVelocity(Vector2 velocity)
	{
		Velocity = velocity;
		_enableGravity = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;
		if (_enableGravity)
		{
			velocity.Y += _gravity * (float)delta;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
