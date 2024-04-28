using System;
using ColdMint.scripts.camp;
using ColdMint.scripts.character;
using ColdMint.scripts.damage;
using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.projectile;

/// <summary>
/// <para>Projectile template</para>
/// <para>抛射体模板</para>
/// </summary>
public partial class ProjectileTemplate : CharacterBody2D
{
	protected Timer _timer;

	protected double _life;

	//The durability of the projectile
	//抛射体的耐久度
	//When the projectile hits the object, the durability will be reduced, and when the durability is less than or equal to 0, the projectile will be destroyed
	//当抛射体撞击到物体时，会减少耐久度，当耐久度小于等于0时，销毁抛射体
	protected double _durability;

	protected int _maxDamage;
	protected int _minDamage;
	protected int _damageType;

	//We use the Time node to specify when to destroy the projectile
	//我们用Time节点来指定何时销毁抛射体
	private Timer timer;

	/// <summary>
	/// <para>The impact area of the bullet</para>
	/// <para>子弹的碰撞区域</para>
	/// </summary>
	protected Area2D _area2D;

	/// <summary>
	/// <para>knockback</para>
	/// <para>击退</para>
	/// </summary>
	/// <remarks>
	///<para>How much force does it have when hitting the character? Unit: Number of cells，The X direction of the force is inferred automatically.</para>
	///<para>当击中角色时带有多大的力？单位：格数，力的X方向是自动推断的。</para>
	/// </remarks>
	protected Vector2 _knockbackForce;

	public float Speed
	{
		get => GetMeta("Speed", "15").AsSingle();
	}

	/// <summary>
	/// <para>The master of the weapon</para>
	/// <para>武器的主人</para>
	/// </summary>
	public Node2D Owner { get; set; }

	public override void _Ready()
	{
		//子弹的碰撞检测区域
		_area2D = GetNode<Area2D>("CollisionDetectionArea");
		_area2D.Monitoring = true;
		_area2D.BodyEntered += OnBodyEnter;
		_area2D.BodyExited += OnBodyExited;
		_durability = GetMeta("Durability", "1").AsDouble();
		_maxDamage = GetMeta("MaxDamage", "7").AsInt32();
		_minDamage = GetMeta("MinDamage", "5").AsInt32();
		_damageType = GetMeta("DamageType", Config.DamageType.Physical).AsInt32();
		_knockbackForce = GetMeta("Knockback", Vector2.Zero).AsVector2();
		//子弹的存在时间
		_life = GetMeta("Life", "10").AsDouble();
		//如果存在时间小于等于0，那么设置为存在10秒，禁止无限期存在的抛射体
		if (_life <= 0)
		{
			_life = 10;
		}

		_timer = new Timer();
		AddChild(_timer);
		_timer.WaitTime = _life;
		_timer.OneShot = true;
		_timer.Start();
		_timer.Timeout += OnTimeOut;
	}


	/// <summary>
	/// <para>Detect whether harm is allowed</para>
	/// <para>检测是否允许造成伤害</para>
	/// </summary>
	/// <param name="owner"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	private bool CanCauseHarm(Node2D owner, Node2D target)
	{
		//We must know who the owner of the bullet is in order to determine whether it should cause damage or not
		//我们必须知道子弹的主人是谁，才能判断是否应该造成伤害
		if (owner == null)
		{
			return false;
		}

		if (owner is not CharacterTemplate ownerCharacterTemplate)
		{
			return false;
		}

		if (target is TileMap)
		{
			//When we hit the tile, we return true in order to place the bullet through the tile.
			//撞击到瓦片时，我们返回true，是为了放置子弹穿透瓦片。
			return true;
		}

		if (target is not CharacterTemplate characterTemplate)
		{
			return false;
		}

		//First get the owner's camp and compare it with the target camp
		//先获取主人的阵营与目标阵营进行比较
		var canCauseHarm = CampManager.CanCauseHarm(CampManager.GetCamp(ownerCharacterTemplate.CampId),
			CampManager.GetCamp(characterTemplate.CampId));
		return canCauseHarm;
	}

	/// <summary>
	/// <para>Executive injury treatment</para>
	/// <para>执行伤害处理</para>
	/// </summary>
	/// <param name="owner"></param>
	/// <param name="target"></param>
	private void DoDamage(Node2D owner, Node2D target)
	{
		if (target is not CharacterTemplate characterTemplate)
		{
			return;
		}

		//Allow damage to be caused
		//允许造成伤害
		var damage = new Damage();
		damage.Attacker = owner;
		damage.MaxDamage = _maxDamage;
		damage.MinDamage = _minDamage;
		damage.CreateDamage();
		damage.MoveLeft = Velocity.X < 0;
		damage.Type = _damageType;
		characterTemplate.Damage(damage);
		if (_knockbackForce != Vector2.Zero)
		{
			//If we set the attack force, then apply the force to the object
			//如果我们设置了攻退力，那么将力应用到对象上
			var force = new Vector2();
			var forceX = Math.Abs(_knockbackForce.X);
			if (Velocity.X < 0)
			{
				//Beat back to port
				//向左击退
				forceX = -forceX;
			}

			force.X = forceX * Config.CellSize;
			force.Y = _knockbackForce.Y * Config.CellSize;
			characterTemplate.AddForce(force);
		}
	}

	/// <summary>
	/// <para>When the bullet is in contact with the node</para>
	/// <para>当子弹与节点接触时</para>
	/// </summary>
	/// <param name="node"></param>
	protected virtual void OnBodyEnter(Node2D node)
	{
		//Here we test whether harm is allowed, notice that for TileMap, we directly allow harm.
		//这里我们检测是否允许造成伤害，注意对于TileMap，我们直接允许造成伤害。
		var canCauseHarm = CanCauseHarm(Owner, node);
		if (!canCauseHarm)
		{
			return;
		}
		DoDamage(Owner, node);
		//Please specify in the Mask who the bullet will collide with
		//请在Mask内配置子弹会和谁碰撞
		//When a bullet hits an object, its durability decreases
		//子弹撞击到物体时，耐久度减少
		_durability--;
		if (_durability <= 0)
		{
			//When the durability is less than or equal to 0, destroy the bullet
			//当耐久度小于等于0时，销毁子弹
			QueueFree();
		}
	}

	/// <summary>
	/// <para>When the bullet leaves the node</para>
	/// <para>当子弹离开节点时</para>
	/// </summary>
	/// <param name="node"></param>
	protected virtual void OnBodyExited(Node2D node)
	{
	}


	/// <summary>
	/// <para>When beyond the time of existence</para>
	/// <para>当超过存在时间</para>
	/// </summary>
	private void OnTimeOut()
	{
		QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}
}
