using System;
using System.Collections.Generic;
using ColdMint.scripts.camp;
using ColdMint.scripts.damage;
using ColdMint.scripts.debug;
using ColdMint.scripts.health;
using ColdMint.scripts.weapon;
using Godot;

namespace ColdMint.scripts.character;

/// <summary>
/// <para>CharacterTemplate</para>
/// <para>角色模板</para>
/// </summary>
/// <remarks>
///<para>Behavior shared by all characters</para>
///<para>所有角色共有的行为</para>
/// </remarks>
public partial class CharacterTemplate : CharacterBody2D
{
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public const float Speed = 300.0f;

	public const float JumpVelocity = -240;

	protected string _characterName;

	public string CharacterName => _characterName;

	//当前持有的物品
	public Node2D CurrentItem;


	//定义一个拾起范围
	private Area2D PickingArea;

	protected AnimatedSprite2D AnimatedSprite2D;

	//一个标志，定义物品的位置
	protected Marker2D ItemMarker2D;

	//物品标记的原始X坐标
	private float ItemMarkerOriginalX;

	public float ReadOnlyItemMarkerOriginalX => ItemMarkerOriginalX;

	//面向左边吗
	public bool FacingLeft = false;

	//The force added by the AddForce method
	//由AddForce方法追加的力
	private Vector2 additionalForce = Vector2.Zero;

	protected int CurrentHp;

	//The initial health of the character after creation
	//角色创建后的初始血量
	private int InitialHp;

	protected int MaxHp;

	/// <summary>
	/// <para>The camp ID of the role</para>
	/// <para>角色的阵营ID</para>
	/// </summary>
	public string CampId;

	private DamageNumberNodeSpawn DamageNumber;

	private HealthBar _healthBar;
	private DateTime _lastDamageTime;

	/// <summary>
	/// <para>Pick up all items within range</para>
	/// <para>拾捡范围内的所有物品</para>
	/// </summary>
	private List<Node> _pickingRangeBodies;

	public Node[] PickingRangeBodies => _pickingRangeBodies.ToArray();


	/// <summary>
	/// <para>Get all weapons within range of the pick up</para>
	/// <para>获取所有在拾捡范围内的武器</para>
	/// </summary>
	/// <returns></returns>
	public WeaponTemplate[] GetCanPickedWeapon()
	{
		var weaponTemplates = new List<WeaponTemplate>();
		foreach (var pickingRangeBody in PickingRangeBodies)
		{
			if (pickingRangeBody is not WeaponTemplate weaponTemplate) continue;
			if (weaponTemplate.Owner != null)
			{
				continue;
			}

			weaponTemplates.Add(weaponTemplate);
		}

		return weaponTemplates.ToArray();
	}

	public override void _Ready()
	{
		base._Ready();
		_pickingRangeBodies = new List<Node>();
		_characterName = GetMeta("Name", Name).AsString();
		CampId = GetMeta("CampId", Config.CampId.Default).AsString();
		MaxHp = GetMeta("MaxHp", Config.DefaultMaxHp).AsInt32();
		if (MaxHp <= 0)
		{
			//若最大血量为0或小于0，则将最大血量设置为10
			MaxHp = Config.DefaultMaxHp;
		}

		InitialHp = GetMeta("InitialHp", "0").AsInt32();
		if (InitialHp <= 0)
		{
			//若初始血量小于等于0，则将初始血量设置为最大血量
			InitialHp = MaxHp;
		}

		CurrentHp = InitialHp;
		//生物的健康条，可能为null。
		_healthBar = GetNodeOrNull<HealthBar>("HealthBar");
		if (_healthBar != null)
		{
			_healthBar.MaxValue = MaxHp;
		}


		ItemMarker2D = GetNode<Marker2D>("ItemMarker2D");
		ItemMarkerOriginalX = ItemMarker2D.Position.X;
		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		PickingArea = GetNode<Area2D>("Area2DPickingArea");
		DamageNumber = GetNode<Marker2D>("DamageNumber") as DamageNumberNodeSpawn;
		if (PickingArea != null)
		{
			//如果为true，该区域将检测进出该区域的物体或区域。
			PickingArea.Monitoring = true;
			//Other regions cannot detect our pick region
			//其他区域不能检测到我们的拾取区域
			PickingArea.Monitorable = false;
			PickingArea.BodyEntered += EnterThePickingRangeBody;
			PickingArea.BodyExited += ExitThePickingRangeBody;
		}
	}

	/// <summary>
	/// <para>Pick up the specified items</para>
	/// <para>将指定物品拾起来</para>
	/// </summary>
	/// <param name="pickAbleItem"></param>
	/// <returns>
	///<para>Whether successfully picked up</para>
	///<para>是否成功拾起</para>
	/// </returns>
	public bool PickItem(Node2D pickAbleItem)
	{
		if (pickAbleItem == null)
		{
			return false;
		}

		if (CurrentItem == null)
		{
			if (pickAbleItem is WeaponTemplate weaponTemplate)
			{
				if (weaponTemplate.Owner != null && weaponTemplate.Owner != this)
				{
					return false;
				}

				weaponTemplate.Owner = this;
				weaponTemplate.SetCollisionMaskValue(Config.LayerNumber.Platform, false);
				weaponTemplate.SetCollisionMaskValue(Config.LayerNumber.Ground, false);
				weaponTemplate.EnableContactInjury = false;
				weaponTemplate.Sleeping = true;
			}

			pickAbleItem.Reparent(ItemMarker2D);
			pickAbleItem.Position = Vector2.Zero;
			CurrentItem = pickAbleItem;
			return true;
		}

		return false;
	}

	/// <summary>
	/// <para>Use what you have in your hand</para>
	/// <para>使用手中的物品</para>
	/// </summary>
	public bool UseItem(Vector2 position)
	{
		if (CurrentItem == null)
		{
			return false;
		}

		if (CurrentItem is WeaponTemplate weaponTemplate)
		{
			weaponTemplate.Fire(this, position);
		}

		return true;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		//如果上次受到伤害的时间与当前时间的时间差大于健康条显示时间，则隐藏健康条
		var timeSpan = DateTime.Now - _lastDamageTime;
		if (timeSpan > Config.HealthBarDisplaysTime)
		{
			if (_healthBar != null)
			{
				_healthBar.Visible = false;
			}
		}
	}

	/// <summary>
	/// <para>Update the role's health bar</para>
	/// <para>更新角色的健康条</para>
	/// </summary>
	protected void UpDataHealthBar(DamageTemplate damageTemplate)
	{
		if (_healthBar == null)
		{
			return;
		}

		if (GameSceneNodeHolder.Player == null)
		{
			//We didn't know who the player was, so we showed it as a hostile color
			//我们不知道玩家是谁，所以我们将其显示为敌对颜色
			_healthBar.SetEnemyTones();
		}
		else
		{
			//If we set up a player node, then compare the injured party ID to the player's party ID
			//如果我们设置了玩家节点，那么将受伤者的阵营ID与玩家的阵营ID进行比较
			var targetCamp = CampManager.GetCamp(CampId);
			var playerCamp = CampManager.GetCamp(GameSceneNodeHolder.Player.CampId);
			if (CampManager.CanCauseHarm(targetCamp, playerCamp))
			{
				if (targetCamp.ID == playerCamp.ID)
				{
					//If an attack is allowed and you are on the same side, it is displayed as a friendly color (friend damage).
					//如果允许攻击，且属于同一阵营，则显示为友好颜色（友伤）
					_healthBar.SetFriendlyTones();
				}
				else
				{
					//If the injured target is an enemy of the player, it is displayed as an enemy color
					//如果受伤的目标是玩家的敌人，则显示为敌对颜色
					_healthBar.SetEnemyTones();
				}
			}
			else
			{
				_healthBar.SetFriendlyTones();
			}
		}

		_healthBar.Visible = true;
		_healthBar.Value = CurrentHp;
	}

	/// <summary>
	/// <para>Deal damage to the character</para>
	/// <para>对角色造成伤害</para>
	/// </summary>
	/// <param name="damageTemplate">
	///<para>Damage template</para>
	///<para>伤害模板</para>
	/// </param>
	/// <returns>
	///<para>Return whether the damage was done successfully</para>
	///<para>返回是否成功造成了伤害</para>
	/// </returns>
	public bool Damage(DamageTemplate damageTemplate)
	{
		_lastDamageTime = DateTime.Now;
		DamageNumber.Display(damageTemplate);
		CurrentHp -= damageTemplate.Damage;
		OnHit(damageTemplate);
		if (CurrentHp <= 0)
		{
			//角色死亡
			OnDie(damageTemplate);
			return true;
		}

		UpDataHealthBar(damageTemplate);
		return true;
	}

	/// <summary>
	/// <para>Add power to the character</para>
	/// <para>在角色身上添加力</para>
	/// </summary>
	/// <param name="force"></param>
	public void AddForce(Vector2 force)
	{
		additionalForce = force;
	}

	protected virtual void OnHit(DamageTemplate damageTemplate)
	{
	}

	/// <summary>
	/// <para>Handle the event of character death</para>
	/// <para>处理角色死亡的事件</para>
	/// </summary>
	/// <param name="damageTemplate"></param>
	protected virtual void OnDie(DamageTemplate damageTemplate)
	{
		QueueFree();
	}

	/// <summary>
	/// <para>When an object enters the picking range</para>
	/// <para>当有物体进入拾捡范围时</para>
	/// </summary>
	/// <param name="node"></param>
	protected virtual void EnterThePickingRangeBody(Node node)
	{
		_pickingRangeBodies.Add(node);
	}

	/// <summary>
	/// <para>When an object exit the picking range</para>
	/// <para>当有物体离开拾捡范围时</para>
	/// </summary>
	/// <param name="node"></param>
	protected virtual void ExitThePickingRangeBody(Node node)
	{
		_pickingRangeBodies.Remove(node);
	}

	/// <summary>
	/// <para>Flip sprites or animations</para>
	/// <para>翻转精灵或动画</para>
	/// </summary>
	protected virtual void Flip()
	{
		AnimatedSprite2D.FlipH = FacingLeft;
	}


	public sealed override void _PhysicsProcess(double delta)
	{
		//We continuously set the position of the items to prevent them from changing as we zoom in and out of the window.
		//我们持续设置物品的位置，为了防止放大缩小窗口时物品位置的变化。
		if (CurrentItem != null)
		{
			CurrentItem.Position = Vector2.Zero;
		}

		var velocity = Velocity;
		// Add the gravity.
		//增加重力。
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;
		// The ref keyword passes its pointer to the following method so that it can be modified in the method.
		// ref关键字将其指针传递给下面的方法，以便在方法中修改它。
		HookPhysicsProcess(ref velocity, delta);
		Velocity = velocity + additionalForce;
		additionalForce = Vector2.Zero;
		MoveAndSlide();
	}

	/// <summary>
	/// <para>Aim the held item at a point</para>
	/// <para>使持有的物品瞄准某个点</para>
	/// </summary>
	public void AimTheCurrentItemAtAPoint(Vector2 position)
	{
		if (CurrentItem == null)
		{
			//Do not currently hold any items.
			//当前没有持有任何物品。
			return;
		}

		// 将旋转角度应用于节点
		CurrentItem.LookAt(position);
	}


	protected virtual void HookPhysicsProcess(ref Vector2 velocity, double delta)
	{
	}
}
