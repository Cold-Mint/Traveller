using System;
using System.Collections.Generic;
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
    [Export] public bool IgnoreWall;

    private List<IProjectileDecorator>? _projectileDecorators;

    /// <summary>
    /// <para>Supports decorators that handle physical frames</para>
    /// <para>支持处理物理帧的装饰器</para>
    /// </summary>
    private List<IProjectileDecorator>? _physicalFrameDecorators;

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
        //If the existence time is less than or equal to 0, then it is set to exist for 10 seconds, and projectiles that exist indefinitely are prohibited
        //如果存在时间小于等于0，那么设置为存在10秒，禁止无限期存在的抛射体
        if (_life <= 0)
        {
            _life = 10000;
        }

        _destructionTime = DateTime.Now.AddMilliseconds(_life);
        SetCollisionLayerValue(Config.LayerNumber.Projectile, true);
        SetCollisionMaskValue(Config.LayerNumber.Wall, !IgnoreWall);
        SetCollisionMaskValue(Config.LayerNumber.Floor, !IgnoreWall);
        SetCollisionMaskValue(Config.LayerNumber.Player, true);
        SetCollisionMaskValue(Config.LayerNumber.Mob, true);
        SetCollisionMaskValue(Config.LayerNumber.PickAbleItem, true);
        SetCollisionMaskValue(Config.LayerNumber.Barrier, true);
        SetCollisionMaskValue(Config.LayerNumber.ProjectileBarrier, true);
        //Platform collision layer is not allowed to collide
        //平台碰撞层不可碰撞
        SetCollisionMaskValue(Config.LayerNumber.Platform, false);
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
        if (decorator.SupportedModificationPhysicalFrame)
        {
            _physicalFrameDecorators ??= [];
            _physicalFrameDecorators.Add(decorator);
        }
        decorator.Attach(this);

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
        if (_projectileDecorators == null)
        {
            return false;
        }
        if (_projectileDecorators.Contains(decorator))
        {
            decorator.Detach(this);
        }
        if (_physicalFrameDecorators != null)
        {
            _physicalFrameDecorators.Remove(decorator);
        }
        return _projectileDecorators.Remove(decorator);
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
            var damage = new RangeDamage
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
            var damage = new RangeDamage
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
    /// <para>Invoke Physical Frame Decorators</para>
    /// <para>调用物理帧装饰器</para>
    /// </summary>
    /// <param name="projectileDecoratorAction"></param>
    private void InvokePhysicalFrameDecorators(Action<IProjectileDecorator> projectileDecoratorAction)
    {
        if (_physicalFrameDecorators == null)
        {
            return;
        }

        foreach (var decorator in _physicalFrameDecorators)
        {
            projectileDecoratorAction(decorator);
        }
    }

    /// <summary>
    /// <para>When beyond the time of existence</para>
    /// <para>当超过存在时间</para>
    /// </summary>
    public void OnTimeOut()
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
        InvokePhysicalFrameDecorators(decorator =>
        {
            decorator.PhysicsProcess(this, collisionInfo);
        });
        if (collisionInfo == null)
        {
            //No collision.
            //没有撞到任何东西。
            return;
        }
        //Here we test whether harm is allowed, notice that for TileMap, we directly allow harm.
        //这里我们检测是否允许造成伤害，注意对于TileMap，我们直接允许造成伤害。
        var node = (Node2D)collisionInfo.GetCollider();
        var canCauseHarm = DamageUtils.CanCauseHarm(Owner, node);
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