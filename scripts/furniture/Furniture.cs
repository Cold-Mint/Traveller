using ColdMint.scripts.damage;
using Godot;

namespace ColdMint.scripts.furniture;

/// <summary>
/// <para>FurnitureTemplate</para>
/// <para>家具模板</para>
/// </summary>
public partial class Furniture : RigidBody2D
{
    [Export] private int _initialDurability;
    [Export] private int _maxDurability;

    /// <summary>
    /// <para></para>
    /// <para>家具的耐久度</para>
    /// </summary>
    private int _durability;

    public override void _Ready()
    {
        if (_maxDurability <= 0)
        {
            _maxDurability = Config.DefaultMaxDurability;
        }

        if (_initialDurability <= 0 || _initialDurability > _maxDurability)
        {
            _initialDurability = _maxDurability;
        }

        _durability = _initialDurability;
        SetCollisionMaskValue(Config.LayerNumber.Wall, true);
        SetCollisionMaskValue(Config.LayerNumber.Platform, true);
        SetCollisionMaskValue(Config.LayerNumber.Floor, true);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustReleased("push"))
        {
            SetCollisionMaskValue(Config.LayerNumber.Player, false);
        }

        if (Input.IsActionJustPressed("push"))
        {
            SetCollisionMaskValue(Config.LayerNumber.Player, true);
        }
    }

    /// <summary>
    /// <para>This method is called when furniture is damaged</para>
    /// <para>当家具损害时调用此方法</para>
    /// </summary>
    /// <param name="damageTemplate"></param>
    /// <returns>
    ///<para>Return whether the damage completely destroyed the furniture</para>
    ///<para>返回本次伤害是否彻底破坏了家具</para>
    /// </returns>
    public bool Damage(DamageTemplate damageTemplate)
    {
        return false;
    }
}