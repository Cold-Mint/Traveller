using System;
using System.Collections.Generic;
using ColdMint.scripts.camp;
using ColdMint.scripts.character;
using ColdMint.scripts.damage;
using ColdMint.scripts.furniture;
using ColdMint.scripts.pickable;
using ColdMint.scripts.projectile.decorator;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.projectile;

/// <summary>
/// <para>Projectile</para>
/// <para>抛射体</para>
/// </summary>
public partial class Projectile : CharacterBody2D
{
    /// <summary>
    /// <para>life(ms)</para>
    /// <para>子弹的存在时间(毫秒)</para>
    /// </summary>
    [Export] private long _life;

    //The durability of the projectile
    //抛射体的耐久度
    //When the projectile hits the object, the durability will be reduced, and when the durability is less than or equal to 0, the projectile will be destroyed
    //当抛射体撞击到物体时，会减少耐久度，当耐久度小于等于0时，销毁抛射体
    [Export] private double _durability;

    [Export] private int _maxDamage;
    [Export] private int _minDamage;
    [Export] private int _damageType;

    /// <summary>
    /// <para>After this time destroy the projectile</para>
    /// <para>超过此时刻销毁抛射体</para>
    /// </summary>
    private DateTime? _destructionTime;

    private float _actualSpeed;

    /// <summary>
    /// <para>The speed of the bullet</para>
    /// <para>子弹的飞行速度</para>
    /// </summary>
    /// <remarks>
    ///<para>Indicates the number of units moved per second</para>
    ///<para>表示每秒移动的单位格数</para>
    /// </remarks>
    [Export] 
    public float Speed
    {
        get => _actualSpeed / Config.CellSize;
        private set => _actualSpeed = value * Config.CellSize;
    }

    /// <summary>
    /// <para>Get actual speed</para>
    /// <para>获取实际速度</para>
    /// </summary>
    public float ActualSpeed => _actualSpeed;

    /// <summary>
    /// <para>Whether it bounces back after hitting an enemy or a wall</para>
    /// <para>是否撞到敌人或墙壁后反弹</para>
    /// </summary>
    [Export] private bool _enableBounce;

    /// <summary>
    /// <para>Can it penetrate the wall</para>
    /// <para>是否可以穿透墙壁</para>
    /// </summary>
    [Export] private bool _ignoreWall;

    /// <summary>
    /// <para>Enable the tracking of the enemy</para>
    /// <para>启用追踪敌人的功能</para>
    /// </summary>
    [Export] public bool EnableTracking;

    /// <summary>
    /// <para>The target dies and destroys the projectile at the same time</para>
    /// <para>在目标死亡后销毁抛射体</para>
    /// </summary>
    [Export] public bool TargetDiesDestroyProjectile;

    /// <summary>
    /// <para>The target</para>
    /// <para>设置目标</para>
    /// </summary>
    public Node2D? TargetNode { get; set; }

    private List<IProjectileDecorator>? _projectileDecorators;

    /// <summary>
    /// <para>Rays used to detect walls</para>
    /// <para>用于检测墙壁的射线</para>
    /// </summary>
    private RayCast2D? _wallRayCast;

    /// <summary>
    /// <para>Repel strength</para>
    /// <para>击退强度</para>
    /// </summary>
    /// <remarks>
    ///<para>Must be greater than 0, the unit is in format.</para>
    ///<para>必须大于0,单位为格式。</para>
    /// </remarks>
    [Export] private float _repelStrength;

    /// <summary>
    /// <para>The master of the weapon</para>
    /// <para>武器的主人</para>
    /// </summary>
    public new Node2D? Owner { get; set; }


    public override void _Ready()
    {
        if (!_ignoreWall)
        {
            _wallRayCast = new RayCast2D();
            _wallRayCast.SetCollisionMaskValue(Config.LayerNumber.Wall, true);
            _wallRayCast.SetCollisionMaskValue(Config.LayerNumber.Floor, true);
            NodeUtils.CallDeferredAddChild(this, _wallRayCast);
        }

        //If the existence time is less than or equal to 0, then it is set to exist for 10 seconds, and projectiles that exist indefinitely are prohibited
        //如果存在时间小于等于0，那么设置为存在10秒，禁止无限期存在的抛射体
        if (_life <= 0)
        {
            _life = 10000;
        }

        _destructionTime = DateTime.Now.AddMilliseconds(_life);
        SetCollisionMaskValue(Config.LayerNumber.Wall, !_ignoreWall);
        SetCollisionMaskValue(Config.LayerNumber.Floor, !_ignoreWall);
        SetCollisionMaskValue(Config.LayerNumber.Player, true);
        SetCollisionMaskValue(Config.LayerNumber.Mob, true);
        SetCollisionMaskValue(Config.LayerNumber.PickAbleItem, true);
        SetCollisionMaskValue(Config.LayerNumber.Barrier, true);
        //Platform collision layer is not allowed to collide
        //平台碰撞层不可碰撞
        SetCollisionMaskValue(Config.LayerNumber.Platform, false);
        if (TargetNode != null)
        {
            TargetNode.TreeExiting += () =>
            {
                //Clear the trace when the target is destroyed.
                //在目标被销毁的时候清空跟踪。
                TargetNode = null;
                if (TargetDiesDestroyProjectile)
                {
                    OnTimeOut();
                }
            };
        }
    }

    /// <summary>
    /// <para>Add decorator</para>
    /// <para>添加装饰器</para>
    /// </summary>
    /// <param name="decorator">
    ///<para>decorator</para>
    ///<para>装饰器</para>
    /// </param>
    public void AddProjectileDecorator(IProjectileDecorator decorator)
    {
        _projectileDecorators ??= [];
        _projectileDecorators.Add(decorator);
    }

    /// <summary>
    /// <para>Remove decorator</para>
    /// <para>移除装饰器</para>
    /// </summary>
    /// <param name="decorator">
    ///<para>decorator</para>
    ///<para>装饰器</para>
    /// </param>
    /// <returns></returns>
    public bool RemoveProjectileDecorator(IProjectileDecorator decorator)
    {
        return _projectileDecorators?.Remove(decorator) ?? false;
    }

    /// <summary>
    /// <para>Detect whether harm is allowed</para>
    /// <para>检测是否允许造成伤害</para>
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool CanCauseHarm(Node2D? owner, Node2D target)
    {
        //Projectiles are not allowed to directly harm the owner.
        //不允许子弹直接伤害主人。
        if (owner == target)
        {
            return false;
        }
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

        if (target is TileMapLayer)
        {
            //When we hit the tile, we return true to prevent the bullet from penetrating the tile.
            //撞击到瓦片时，我们返回true，是为了防止子弹穿透瓦片。
            return true;
        }

        if (target is Furniture)
        {
            return true;
        }

        if (target is PickAbleTemplate pickAbleTemplate)
        {
            //The picked-up item cannot resist the bullet.
            //被拾起的物品无法抵挡子弹。
            return !pickAbleTemplate.Picked;
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
    private void DoDamage(Node2D? owner, Node2D target)
    {
        if (target is CharacterTemplate characterTemplate)
        {
            //Allow damage to be caused
            //允许造成伤害
            var damage = new Damage
            {
                Attacker = owner,
                MaxDamage = _maxDamage,
                MinDamage = _minDamage
            };
            damage.CreateDamage();
            damage.MoveLeft = Velocity.X < 0;
            damage.Type = _damageType;
            var dead = characterTemplate.Damage(damage);
            if (dead)
            {
                //If the character is dead, then call OnKillCharacter
                //如果角色死亡了，那么调用OnKillCharacter
                InvokeDecorators(decorator => { decorator.OnKillCharacter(owner, characterTemplate); });
            }

            if (_repelStrength > 0)
            {
                //If we set the attack force, then apply the force to the object
                //如果我们设置了攻退力，那么将力应用到对象上
                var normalized = Velocity.Normalized();
                characterTemplate.AddForce(new Vector2(normalized.X * _repelStrength * Config.CellSize,
                    normalized.Y * _repelStrength * Config.CellSize));
            }
        }
        else if (target is PickAbleTemplate pickAbleTemplate)
        {
            if (_repelStrength > 0)
            {
                //If we set the attack force, then apply the force to the object
                //如果我们设置了攻退力，那么将力应用到对象上
                var normalized = Velocity.Normalized();
                pickAbleTemplate.ApplyImpulse(new Vector2(normalized.X * _repelStrength * Config.CellSize,
                    normalized.Y * _repelStrength * Config.CellSize));
            }
        }
        else if (target is Furniture furniture)
        {
            var damage = new Damage
            {
                Attacker = owner,
                MaxDamage = _maxDamage,
                MinDamage = _minDamage
            };
            damage.CreateDamage();
            damage.MoveLeft = Velocity.X < 0;
            damage.Type = _damageType;
            furniture.Damage(damage);
        }
    }

    /// <summary>
    /// <para>Call the method of the decorator</para>
    /// <para>调用装饰器的方法</para>
    /// </summary>
    /// <param name="projectileDecoratorAction"></param>
    private void InvokeDecorators(Action<IProjectileDecorator> projectileDecoratorAction)
    {
        if (_projectileDecorators == null)
        {
            return;
        }

        foreach (var decorator in _projectileDecorators)
        {
            projectileDecoratorAction(decorator);
        }
    }

    /// <summary>
    /// <para>When beyond the time of existence</para>
    /// <para>当超过存在时间</para>
    /// </summary>
    private void OnTimeOut()
    {
        QueueFree();
    }

    public override void _Process(double delta)
    {
        //If the existence time is exceeded, the projectile is destroyed
        //如果超过了存在时间，那么销毁抛射体
        if (DateTime.Now >= _destructionTime)
        {
            OnTimeOut();
        }
        LookAt(GlobalPosition + Velocity);
    }

    public override void _PhysicsProcess(double delta)
    {
        var collisionInfo = MoveAndCollide(Velocity * (float)delta);
        if (collisionInfo == null)
        {
            //No collision.
            //没有撞到任何东西。
            if (EnableTracking && TargetNode != null)
            {
                //Track the target
                //追踪目标
                //Gets a vector of the projectile pointing at the enemy's position.
                //得到抛射体指向敌人位置的向量。
                var desiredVelocity = TargetNode.GlobalPosition - GlobalPosition;
                if (!_ignoreWall && _wallRayCast != null)
                {
                    _wallRayCast!.TargetPosition = desiredVelocity;
                    if (_wallRayCast.IsColliding())
                    {
                        return;
                    }
                }

                var actualDesiredVelocity = desiredVelocity.Normalized() * _actualSpeed;
                //The weight is smaller, the circle is larger.
                //weight越小，子弹绕的圈越大。
                Velocity = Velocity.Lerp(actualDesiredVelocity, 0.1f);
            }
        }
        else
        {
            //Here we test whether harm is allowed, notice that for TileMap, we directly allow harm.
            //这里我们检测是否允许造成伤害，注意对于TileMap，我们直接允许造成伤害。
            var godotObject = collisionInfo.GetCollider();
            var node = (Node2D)godotObject;
            var canCauseHarm = CanCauseHarm(Owner, node);
            if (!canCauseHarm)
            {
                return;
            }

            //Bump into other objects.
            //撞到其他对象。
            if (_enableBounce)
            {
                Velocity = Velocity.Bounce(collisionInfo.GetNormal());
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
    }
}