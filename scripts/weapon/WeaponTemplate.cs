using System;
using ColdMint.scripts.camp;
using ColdMint.scripts.character;
using ColdMint.scripts.damage;
using ColdMint.scripts.inventory;
using Godot;

namespace ColdMint.scripts.weapon;

/// <summary>
/// <para>WeaponTemplate</para>
/// <para>武器模板</para>
/// </summary>
public partial class WeaponTemplate : RigidBody2D, IItem
{
    public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public string? Id { get; set; }
    public int Quantity { get; set; }
    public int MaxStackQuantity { get; set; }
    public Texture2D? Icon { get; set; }
    public new string? Name { get; set; }
    public string? Description { get; set; }
    public Action<IItem>? OnUse { get; set; }
    public Func<IItem, Node>? OnInstantiation { get; set; }

    /// <summary>
    /// <para>Owner</para>
    /// <para>主人</para>
    /// </summary>
    public new Node2D? Owner { get; set; }


    /// <summary>
    /// <para>Enabled contact injury</para>
    /// <para>启用接触伤害</para>
    /// </summary>
    public bool EnableContactInjury;

    private int _minContactInjury;
    private int _maxContactInjury;

    private DateTime? _lastFiringTime;

    //开火间隔
    private TimeSpan _firingInterval;


    /// <summary>
    /// <para>The recoil of the weapon</para>
    /// <para>武器的后坐力</para>
    /// </summary>
    /// <remarks>
    ///<para>When the weapon is fired, how much recoil is applied to the user, in units: the number of cells, and the X direction of the force is automatically inferred.</para>
    ///<para>武器开火，要对使用者施加多大的后坐力，单位：格数，力的X方向是自动推断的。</para>
    /// </remarks>
    private Vector2 _recoil;

    private Area2D? _area2D;

    protected RayCast2D? RayCast2D;


    public override void _Ready()
    {
        RayCast2D = GetNode<RayCast2D>("RayCast2D");
        _area2D = GetNode<Area2D>("Area2D");
        _area2D.BodyEntered += OnBodyEnter;
        Id = GetMeta("ID", "1").AsString();
        Quantity = GetMeta("Quantity", "1").AsInt32();
        MaxStackQuantity = GetMeta("MaxStackQuantity", Config.MaxStackQuantity).AsInt32();
        Icon = GetMeta("Icon", "").As<Texture2D>();
        Name = GetMeta("Name", "").AsString();
        Description = GetMeta("Description", "").AsString();
        _firingInterval = TimeSpan.FromMilliseconds(GetMeta("FiringInterval", "100").AsInt64());
        _minContactInjury = GetMeta("MinContactInjury", "1").AsInt32();
        _maxContactInjury = GetMeta("MaxContactInjury", "2").AsInt32();
        _recoil = GetMeta("Recoil", Vector2.Zero).AsVector2();
    }


    /// <summary>
    /// <para>Use weapons against the enemy</para>
    /// <para>使用武器砸敌人</para>
    /// </summary>
    /// <param name="node"></param>
    private void OnBodyEnter(Node node)
    {
        if (!EnableContactInjury)
        {
            return;
        }

        if (Owner == null)
        {
            return;
        }

        if (Owner is not CharacterTemplate ownerCharacterTemplate)
        {
            return;
        }

        if (node is not CharacterTemplate characterTemplate)
        {
            return;
        }

        //Determine if your side can cause damage
        //判断所属的阵营是否可以造成伤害
        var canCauseHarm = CampManager.CanCauseHarm(CampManager.GetCamp(ownerCharacterTemplate.CampId),
            CampManager.GetCamp(characterTemplate.CampId));
        if (!canCauseHarm)
        {
            return;
        }

        //If allowed to cause harm
        //如果允许造成伤害
        Owner = null;
        var damage = new Damage
        {
            MaxDamage = Math.Abs(_maxContactInjury),
            MinDamage = Math.Abs(_minContactInjury)
        };
        damage.Attacker = ownerCharacterTemplate;
        damage.CreateDamage();
        damage.MoveLeft = LinearVelocity.X < 0;
        damage.Type = Config.DamageType.Physical;
        characterTemplate.Damage(damage);
    }

    /// <summary>
    /// <para>翻转武器</para>
    /// </summary>
    /// <param name="facingLeft"></param>
    public void Flip(bool facingLeft)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (RayCast2D != null)
        {
            if (RayCast2D.IsColliding())
            {
                //If the weapon hits the ground, we disable physical damage.
                //如果武器落到地面了，我们禁用物理伤害。
                EnableContactInjury = false;
                //Items can be pushed by the player when they are on the ground
                //当物品在地面上时，可被玩家推动
                SetCollisionMaskValue(Config.LayerNumber.Player, true);
            }
            else
            {
                SetCollisionMaskValue(Config.LayerNumber.Player, false);
            }
        }
    }


    /// <summary>
    /// <para>Discharge of the weapon</para>
    /// <para>武器开火</para>
    /// </summary>
    /// <remarks>
    ///<param name="owner">
    ///<para>owner</para>
    ///<para>武器所有者</para>
    /// </param>
    /// <param name="enemyGlobalPosition">
    ///<para>enemyGlobalPosition</para>
    ///<para>敌人所在位置</para>
    /// </param>
    /// </remarks>
    public void Fire(Node2D? owner, Vector2 enemyGlobalPosition)
    {
        var nowTime = DateTime.Now;
        if (_lastFiringTime == null || nowTime - _lastFiringTime > _firingInterval)
        {
            if (owner is CharacterTemplate characterTemplate)
            {
                //我们在每次开火之前，检查武器的后坐力。
                if (_recoil != Vector2.Zero)
                {
                    //假设此武器拥有后坐力
                    var force = new Vector2();
                    var forceX = Math.Abs(_recoil.X);
                    if (Math.Abs(RotationDegrees) < 90)
                    {
                        //The weapon goes to the right and we apply a recoil to the left
                        //武器朝向右边我们向左施加后坐力
                        forceX = -forceX;
                    }

                    force.X = forceX * Config.CellSize;
                    force.Y = _recoil.Y * Config.CellSize;
                    characterTemplate.AddForce(force);
                }
            }

            //If the time difference is greater than the firing interval, then fire
            //如果可以时间差大于开火间隔，那么开火
            DoFire(owner, enemyGlobalPosition);
            _lastFiringTime = nowTime;
        }
    }

    /// <summary>
    /// <para>Execute fire</para>
    /// <para>执行开火</para>
    /// </summary>
    protected virtual void DoFire(Node2D? owner, Vector2 enemyGlobalPosition)
    {
    }
}