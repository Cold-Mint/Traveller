using System.Collections.Generic;
using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.damage;

/// <summary>
/// <para>Node representing the damage number</para>
/// <para>表示伤害数字的节点</para>
/// </summary>
public partial class DamageNumberNodeSpawn : Marker2D
{
	private PackedScene _damageNumberPackedScene;
	private Node2D _rootNode;

	/// <summary>
	/// <para>The horizontal velocity is in the X positive direction</para>
	/// <para>水平速度的X正方向</para>
	/// </summary>
	private int horizontalVelocityPositiveDirection;

	/// <summary>
	/// <para>Horizontal velocity in the negative X direction</para>
	/// <para>水平速度的X负方向</para>
	/// </summary>
	private int horizontalVelocityNegativeDirection;

	/// <summary>
	/// <para>vertical height</para>
	/// <para>垂直高度</para>
	/// </summary>
	private int verticalHeight;

	/// <summary>
	/// <para>物理渐变色</para>
	/// <para>physical Gradient</para>
	/// </summary>
	private Gradient physicalGradient;

	/// <summary>
	/// <para>魔法渐变色</para>
	/// <para>magic Gradient</para>
	/// </summary>
	private Gradient magicGradient;

	/// <summary>
	/// <para>默认渐变色</para>
	/// <para>default Gradient</para>
	/// </summary>
	private Gradient defaultGradient;


	public override void _Ready()
	{
		base._Ready();
		_damageNumberPackedScene = GD.Load("res://prefab/ui/DamageNumber.tscn") as PackedScene;
		_rootNode = GetNode<Node2D>("/root/Game/DamageNumberContainer");
		horizontalVelocityPositiveDirection = Config.CellSize * Config.HorizontalSpeedOfDamageNumbers;
		horizontalVelocityNegativeDirection = -horizontalVelocityPositiveDirection;
		verticalHeight = -Config.CellSize * Config.VerticalVelocityOfDamageNumbers;
		physicalGradient = new Gradient();
		//物理色 从OpenColor2 到 OpenColor6（红色）
		physicalGradient.SetColor(0, new Color("#ffc9c9"));
		physicalGradient.SetColor(1, new Color("#fa5252"));
		magicGradient = new Gradient();
		//魔法色 从OpenColor2 到 OpenColor6(紫色)
		magicGradient.SetColor(0, new Color("#d0bfff"));
		magicGradient.SetColor(1, new Color("#7950f2"));
		defaultGradient = new Gradient();
		//默认行为
		defaultGradient.SetColor(0, new Color("#ff8787"));
		defaultGradient.SetColor(1, new Color("#fa5252"));
	}

	/// <summary>
	/// <para>Added a damage digital node</para>
	/// <para>添加伤害数字节点</para>
	/// </summary>
	/// <param name="damageNumber"></param>
	private void AddDamageNumberNode(Node2D damageNumber)
	{
		_rootNode.AddChild(damageNumber);
	}

	/// <summary>
	/// <para>Show damage</para>
	/// <para>显示伤害</para>
	/// </summary>
	/// <param name="damageTemplate"></param>
	public void Display(DamageTemplate damageTemplate)
	{
		if (_damageNumberPackedScene == null)
		{
			return;
		}

		if (_damageNumberPackedScene.Instantiate() is not DamageNumber damageNumber)
		{
			return;
		}

		CallDeferred("AddDamageNumberNode", damageNumber);

		damageNumber.Position = GlobalPosition;
		if (damageTemplate.MoveLeft)
		{
			damageNumber.SetVelocity(new Vector2(horizontalVelocityNegativeDirection, verticalHeight));
		}
		else
		{
			damageNumber.SetVelocity(new Vector2(horizontalVelocityPositiveDirection, verticalHeight));
		}

		var damageLabel = damageNumber.GetNode<Label>("Label");
		damageLabel.Text = damageTemplate.Damage.ToString();
		var labelSettings = new LabelSettings();
		var offset = damageTemplate.Damage / (float)damageTemplate.MaxDamage;
		labelSettings.FontColor = GetDamageColorByType(damageTemplate.Type).Sample(offset);
		if (damageTemplate.IsCriticalStrike)
		{
			labelSettings.FontSize = Config.CritDamageTextSize;
		}
		else
		{
			labelSettings.FontSize = Config.NormalDamageTextSize;
		}

		damageLabel.LabelSettings = labelSettings;
		damageLabel.Position = Vector2.Zero;
	}


	/// <summary>
	/// <para>Gets text color based on damage type</para>
	/// <para>根据伤害类型获取文本颜色</para>
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	private Gradient GetDamageColorByType(int type)
	{
		return type switch
		{
			Config.DamageType.Physical => physicalGradient,
			Config.DamageType.Magic => magicGradient,
			_ => defaultGradient
		};
	}
}
