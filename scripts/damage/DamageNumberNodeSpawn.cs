using ColdMint.scripts.heal;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.damage;

/// <summary>
/// <para>Node representing the damage number</para>
/// <para>表示伤害数字的节点</para>
/// </summary>
public partial class DamageNumberNodeSpawn : Marker2D
{
    private PackedScene? _damageNumberPackedScene;
    private Node2D? _rootNode;

    /// <summary>
    /// <para>The vector in the negative direction</para>
    /// <para>负方向的向量</para>
    /// </summary>
    private Vector2 _negativeVector;

    /// <summary>
    /// <para>Vector in the positive direction</para>
    /// <para>正方向的向量</para>
    /// </summary>
    private Vector2 _positiveVector;

    /// <summary>
    /// <para>物理渐变色</para>
    /// <para>physical Gradient</para>
    /// </summary>
    private Gradient? _physicalGradient;

    /// <summary>
    /// <para>health Color</para>
    /// <para>治疗颜色</para>
    /// </summary>
    private Color _healthColor;

    /// <summary>
    /// <para>魔法渐变色</para>
    /// <para>magic Gradient</para>
    /// </summary>
    private Gradient? _magicGradient;

    /// <summary>
    /// <para>默认渐变色</para>
    /// <para>default Gradient</para>
    /// </summary>
    private Gradient? _defaultGradient;


    public override void _Ready()
    {
        base._Ready();
        _damageNumberPackedScene = ResourceLoader.Load("res://prefab/ui/DamageNumber.tscn") as PackedScene;
        _rootNode = GetNode<Node2D>("/root/Game/DamageNumberContainer");
        //The horizontal velocity is in the X positive direction
        //水平速度的X正方向
        var horizontalVelocityPositiveDirection = Config.CellSize * Config.HorizontalSpeedOfDamageNumbers;
        //Horizontal velocity in the negative X direction
        //水平速度的X负方向
        var horizontalVelocityNegativeDirection = -horizontalVelocityPositiveDirection;
        //vertical height
        //垂直高度
        var verticalHeight = -Config.CellSize * Config.VerticalVelocityOfDamageNumbers;
        //Compute left and right vectors
        //计算左右向量
        _negativeVector = new Vector2(horizontalVelocityNegativeDirection, verticalHeight);
        _positiveVector = new Vector2(horizontalVelocityPositiveDirection, verticalHeight);
        _physicalGradient = new Gradient();
        //Physical color from OpenColor2 to OpenColor6 (red)
        //物理色 从OpenColor2 到 OpenColor6（红色）
        _physicalGradient.SetColor(0, new Color("#ffc9c9"));
        _physicalGradient.SetColor(1, new Color("#fa5252"));
        _magicGradient = new Gradient();
        //Magic Color from OpenColor2 to OpenColor6(Purple)
        //魔法色 从OpenColor2 到 OpenColor6(紫色)
        _magicGradient.SetColor(0, new Color("#d0bfff"));
        _magicGradient.SetColor(1, new Color("#7950f2"));
        _defaultGradient = new Gradient();
        //default behavior
        //默认行为
        _defaultGradient.SetColor(0, new Color("#ff8787"));
        _defaultGradient.SetColor(1, new Color("#fa5252"));
        _healthColor = Colors.Green;
    }


    /// <summary>
    /// <para>DisplayHeal</para>
    /// <para>显示治疗量</para>
    /// </summary>
    /// <param name="heal"></param>
    /// <param name="actualHealAmount">
    ///<para>actualHealAmount</para>
    ///<para>实际治疗量</para>
    /// </param>
    /// <remarks>
    ///<para>For example, if the expected amount of healing is 100 and the player actually has 20 left, the actual amount of healing is 20.</para>
    ///<para>例如：预期的治疗量为100，实际上玩家还有20就满生命了，那么实际治疗量为20。</para>
    /// </remarks>
    public void DisplayHeal(Heal heal, int actualHealAmount)
    {
        if (_rootNode == null || _damageNumberPackedScene == null)
        {
            return;
        }

        var damageNumber = NodeUtils.InstantiatePackedScene<DamageNumber>(_damageNumberPackedScene);
        if (damageNumber == null)
        {
            return;
        }

        if (_rootNode == null)
        {
            damageNumber.QueueFree();
            return;
        }
        NodeUtils.CallDeferredAddChild(_rootNode, damageNumber);
        damageNumber.Position = GlobalPosition;
        if (heal.MoveLeft)
        {
            damageNumber.SetVelocity(_negativeVector);
        }
        else
        {
            damageNumber.SetVelocity(_positiveVector);
        }
        damageNumber.SetVelocity(_negativeVector);
        var damageLabel = damageNumber.GetNode<Label>("Label");
        if (damageLabel == null)
        {
            return;
        }
        damageLabel.Text = actualHealAmount.ToString();
        var labelSettings = new LabelSettings
        {
            FontSize = Config.NormalDamageTextSize,
            FontColor = _healthColor,
        };
        damageLabel.LabelSettings = labelSettings;
        damageLabel.Position = Vector2.Zero;
    }


    /// <summary>
    /// <para>Show damage</para>
    /// <para>显示伤害</para>
    /// </summary>
    /// <param name="damage"></param>
    public void DisplayDamage(IDamage damage)
    {
        if (_rootNode == null || _damageNumberPackedScene == null)
        {
            return;
        }

        var damageNumber = NodeUtils.InstantiatePackedScene<DamageNumber>(_damageNumberPackedScene);
        if (damageNumber == null)
        {
            return;
        }

        if (_rootNode == null)
        {
            damageNumber.QueueFree();
            return;
        }
        NodeUtils.CallDeferredAddChild(_rootNode, damageNumber);
        damageNumber.Position = GlobalPosition;
        if (damage.MoveLeft)
        {
            damageNumber.SetVelocity(_negativeVector);
        }
        else
        {
            damageNumber.SetVelocity(_positiveVector);
        }

        var damageLabel = damageNumber.GetNode<Label>("Label");
        if (damageLabel == null)
        {
            return;
        }

        damageLabel.Text = damage.Damage.ToString();
        var labelSettings = new LabelSettings();
        var gradient = GetDamageColorByType(damage.Type);
        if (gradient != null && damage is RangeDamage rangeDamage)
        {
            var offset = damage.Damage / (float)rangeDamage.MaxDamage;
            labelSettings.FontColor = gradient.Sample(offset);
        }
        labelSettings.FontSize = damage.IsCriticalStrike ? Config.CritDamageTextSize : Config.NormalDamageTextSize;
        damageLabel.LabelSettings = labelSettings;
        damageLabel.Position = Vector2.Zero;
    }


    /// <summary>
    /// <para>Gets text color based on damage type</para>
    /// <para>根据伤害类型获取文本颜色</para>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private Gradient? GetDamageColorByType(int type)
    {
        return type switch
        {
            Config.DamageType.Physical => _physicalGradient,
            Config.DamageType.Magic => _magicGradient,
            _ => _defaultGradient
        };
    }
}