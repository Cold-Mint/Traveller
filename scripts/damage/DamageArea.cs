using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using ColdMint.scripts.furniture;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.damage;

/// <summary>
/// <para>DamageArea</para>
/// <para>伤害区域</para>
/// </summary>
public partial class DamageArea : Area2D
{

    private RangeDamage? _rangeDamage;
    private int _damageRange;
    private readonly List<CharacterTemplate> _characterTemplates = [];
    private readonly List<Barrier> _barriers = [];
    private CollisionShape2D? _collisionShape2D;
    /// <summary>
    /// <para>Whether it is a one-time injury area</para>
    /// <para>是否为一次性的伤害区域</para>
    /// </summary>
    /// <remarks>
    ///<para>When the remaining number of times is 0, the device automatically destroys</para>
    ///<para>剩余次数为0时，自动销毁</para>
    /// </remarks>
    public bool OneShot { get; set; }
    /// <summary>
    /// <para>residualUse</para>
    /// <para>剩余使用次数</para>
    /// </summary>
    ///<remarks>
    ///<para>When the remaining number of uses is greater than 0, the damage area will pay damage to the creatures entering the area.</para>
    ///<para>当剩余使用次数时大于0时，伤害区域会对进入范围的生物结算伤害。</para>
    /// </remarks>
    private int _residualUse;
    /// <summary>
    /// <para>Whether damage is calculated based on the midpoint of the shape.</para>
    /// <para>是否基于形状的中点计算伤害。</para>
    /// </summary>
    /// <remarks>
    /// <para>Take the circular damage area as an example: if the radius is 50 and the distance from the creature to the midpoint is 45, then the damage received is 45/50=0.9,1-0.9=0.1. This is based on a midpoint calculation. Otherwise based on edge computing. The damage is 45/50=0.9.</para>
    /// <para>以圆形伤害区域为例：半径为50,生物到中点的距离为45,那么收到的伤害为45/50=0.9,1-0.9=0.1。这是基于中点计算的结果。否则基于边缘计算。伤害为45/50=0.9。</para>
    /// </remarks>
    [Export]
    private bool _isDamageCenterBased = true; // skipcq:CS-R1137
    /// <summary>
    /// <para>Damage is caused when it comes into contact with the damaged area(Even if the creature is out of shape)</para>
    /// <para>当接触到伤害区域后便可造成伤害(即使生物在形状外)</para>
    /// </summary>
    [Export]
    private bool _damageOnContact; // skipcq:CS-R1137

    private Node2D? _ownerNode;
    public Node2D? OwnerNode
    {
        get => _ownerNode;
        set
        {
            if (_rangeDamage != null)
            {
                _rangeDamage.Attacker = value;
            }
            _ownerNode = value;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        InputPickable = false;
        SetCollisionMaskValue(Config.LayerNumber.Player, true);
        SetCollisionMaskValue(Config.LayerNumber.Mob, true);
        SetCollisionMaskValue(Config.LayerNumber.Barrier, true);
        Monitoring = true;
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    /// <summary>
    /// <para>Delayed Add Adds the usage times</para>
    /// <para>延迟添加使用次数</para>
    /// </summary>
    /// <param name="additions">
    /// ///<para>Number of additions</para>
    /// ///<para>要添加的次数</para>
    /// </param>
    /// <param name="framesToWait">
    /// ///<para>How many frames do we need to wait?</para>
    /// ///<para>需要等待的帧数</para>
    /// </param>
    public async Task AddResidualUseAsync(int additions, int framesToWait = 1)
    {
        for (var i = 0; i < framesToWait; i++)
        {
            await ToSignal(GetTree(), "process_frame");
        }
        AddResidualUse(additions);
    }


    /// <summary>
    /// <para>Add the remaining times</para>
    /// <para>添加剩余使用次数</para>
    /// </summary>
    /// <param name="additions">
    ///<para>Number of additions</para>
    ///<para>添加的数量</para>
    /// </param>
    public void AddResidualUse(int additions)
    {
        _residualUse += additions;
    }

    /// <summary>
    /// <para>设置伤害参数</para>
    /// <para>Sets the damage parameters</para>
    /// </summary>
    /// <param name="rangeDamage">
    /// <para>区间伤害对象，可以为空</para>
    /// <para>The RangeDamage object, can be null</para>
    /// </param>
    public void SetDamage(RangeDamage? rangeDamage)
    {
        _rangeDamage = rangeDamage;
        if (rangeDamage != null)
        {
            rangeDamage.Attacker = _ownerNode;
            _damageRange = rangeDamage.MaxDamage - rangeDamage.MinDamage;
        }
    }

    /// <summary>
    /// <para>When a player or creature enters the damage zone</para>
    /// <para>当玩家或生物进入伤害区域</para>
    /// </summary>
    /// <param name="body"></param>
    private void OnBodyExited(Node2D body)
    {
        if (body is CharacterTemplate characterTemplate)
        {
            _characterTemplates.Remove(characterTemplate);
        }
        if (body is Barrier barrier)
        {
            _barriers.Remove(barrier);
        }
    }

    /// <summary>
    /// <para>When a player or creature exits the damage area</para>
    /// <para>当玩家或生物退出伤害区域</para>
    /// </summary>
    /// <param name="body"></param>
    private void OnBodyEntered(Node2D body)
    {
        if (body is CharacterTemplate characterTemplate)
        {
            _characterTemplates.Add(characterTemplate);
        }
        if (body is Barrier barrier)
        {
            _barriers.Add(barrier);
        }
    }

    /// <summary>
    /// <para>CreateFixedDamage</para>
    /// <para>创建固定伤害</para>
    /// </summary>
    /// <param name="damage">
    ///<para>damage</para>
    ///<para>伤害</para>
    /// </param>
    /// <returns></returns>
    private FixedDamage? CreateFixedDamage(int damage)
    {
        if (_rangeDamage == null)
        {
            return null;
        }
        return new FixedDamage(damage)
        {
            Type = _rangeDamage.Type,
            Attacker = this,
            IsCriticalStrike = _rangeDamage.GetNewCriticalStrikeStatus()
        };
    }

    /// <summary>
    /// <para>CreateRangeDamage</para>
    /// <para>创建范围伤害</para>
    /// </summary>
    /// <returns></returns>
    private RangeDamage? CreateRangeDamage()
    {
        if (_rangeDamage == null)
        {
            return null;
        }
        return new RangeDamage
        {
            MinDamage = _rangeDamage.MinDamage,
            MaxDamage = _rangeDamage.MaxDamage,
            Type = _rangeDamage.Type,
            CriticalStrikeProbability = _rangeDamage.CriticalStrikeProbability,
            Attacker = this
        };
    }

    private void CircleShape2D<T>(float radius, T node2D, Action<IDamage, T> doDamageAction) where T : Node2D
    {
        if (_rangeDamage == null)
        {
            return;
        }
        if (!DamageUtils.CanCauseHarm(OwnerNode, node2D))
        {
            return;
        }
        var distance = node2D.GlobalPosition.DistanceSquaredTo(node2D.GlobalPosition);
        if (distance > radius)
        {
            //The creature or player is outside the shape.
            //生物或玩家在形状外围。
            if (_damageOnContact)
            {
                //If contact can cause injury.
                //如果接触后即可造成伤害。
                var minFixedDamage = CreateFixedDamage(_rangeDamage.MinDamage);
                if (minFixedDamage != null)
                {
                    doDamageAction.Invoke(minFixedDamage, node2D);
                }
            }
            return;
        }
        var percent = _isDamageCenterBased ? 1 - distance / radius : distance / radius;
        var percentFixedDamage = CreateFixedDamage(_rangeDamage.MinDamage + (int)(_damageRange * percent));
        if (percentFixedDamage != null)
        {
            doDamageAction.Invoke(percentFixedDamage, node2D);
        }
    }

    private void RectangleShape2D<T>(Rect2 rect2, T node2D, Action<IDamage, T> doDamageAction) where T : Node2D
    {
        if (_rangeDamage == null)
        {
            return;
        }
        if (!DamageUtils.CanCauseHarm(OwnerNode, node2D))
        {
            return;
        }
        //Determines whether a coordinate is contained within the rectangle.
        //判断某个坐标是否包含在矩形内。
        if (rect2.HasPoint(node2D.GlobalPosition))
        {
            //The coordinates of the creature are inside the rectangle.
            //生物的坐标在矩形内。
            var rangeDamage = CreateRangeDamage();
            if (rangeDamage != null)
            {
                rangeDamage.CreateDamage();
                doDamageAction.Invoke(rangeDamage, node2D);
            }
        }
        else
        {
            if (_damageOnContact)
            {
                var minFixedDamage = CreateFixedDamage(_rangeDamage.MinDamage);
                if (minFixedDamage != null)
                {
                    doDamageAction.Invoke(minFixedDamage, node2D);
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_residualUse <= 0 || OwnerNode == null || _collisionShape2D == null || _rangeDamage == null)
        {
            return;
        }
        _residualUse--;
        if (_collisionShape2D.Shape is CircleShape2D circleShape2D)
        {
            //CircleShape2D
            //圆形
            var radius = (float)Math.Pow(circleShape2D.Radius, 2);
            foreach (var characterTemplate in _characterTemplates)
            {
                CircleShape2D(radius, characterTemplate, (damage, template) =>
                {
                    template.Damage(damage);
                });
            }
            foreach (var barrier in _barriers)
            {
                CircleShape2D(radius, barrier, (damage, barrier1) =>
                {
                    barrier1.Damage(damage);
                });
            }
        }
        else if (_collisionShape2D.Shape is RectangleShape2D rectangleShape2D)
        {
            //Rectangular shape 2D
            //矩形形状2D
            var rectangleShapeRect = rectangleShape2D.GetRect();
            var rect = new Rect2(new Vector2(rectangleShapeRect.Position.X + _collisionShape2D.GlobalPosition.X, rectangleShapeRect.Position.Y + _collisionShape2D.GlobalPosition.Y), rectangleShapeRect.Size);
            foreach (var characterTemplate in _characterTemplates)
            {
                RectangleShape2D(rect, characterTemplate, (damage, template) =>
                {
                    template.Damage(damage);
                });
            }
            foreach (var barrier in _barriers)
            {
                RectangleShape2D(rect, barrier, (damage, barrier1) =>
                {
                    barrier1.Damage(damage);
                });
            }
        }
        else
        {
            LogCat.LogError("invalid_damage_shape");
        }
        //Determine whether to destroy.
        //判断是否销毁。
        if (OneShot && _residualUse <= 0)
        {
            QueueFree();
        }
    }
}