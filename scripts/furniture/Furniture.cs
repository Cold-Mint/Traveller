using ColdMint.scripts.damage;
using ColdMint.scripts.utils;
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
    [Export]
    private string? _furnitureName;

    private Label? _tipLabel;

    public override void _MouseEnter()
    {
        if (_tipLabel == null || string.IsNullOrEmpty(_furnitureName))
        {
            return;
        }
        var translation = TranslationServerUtils.Translate(_furnitureName);
        if (string.IsNullOrEmpty(translation))
        {
            return;
        }
        _tipLabel.Visible = true;
        _tipLabel.Text = translation;
        //Vertical Centering Tip
        //垂直居中提示
        var oldPosition = _tipLabel.Position;
        oldPosition.X = -_tipLabel.Size.X / 2;
        _tipLabel.Rotation = -Rotation;
        _tipLabel.Position = oldPosition;
    }

    public override void _MouseExit()
    {
        if (_tipLabel == null)
        {
            return;
        }

        _tipLabel.Visible = false;
    }

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
        _tipLabel = GetNodeOrNull<Label>("TipLabel");
        InputPickable = true;
        _durability = _initialDurability;
        SetCollisionLayerValue(Config.LayerNumber.Furniture, true);
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
        _durability -= damageTemplate.Damage;
        if (_durability <= 0)
        {
            QueueFree();
        }
        return true;
    }
}