using System.Collections.Generic;
using ColdMint.scripts.character;
using ColdMint.scripts.debug;
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

    private readonly List<CharacterTemplate> _characterTemplates = new();
    private CollisionShape2D? _collisionShape2D;
    /// <summary>
    /// Whether damage is calculated based on the midpoint of the shape.
    /// 是否基于形状的中点计算伤害。
    /// </summary>
    /// <remarks>
    /// Take the circular damage area as an example: if the radius is 50 and the distance from the creature to the midpoint is 45, then the damage received is 45/50=0.9,1-0.9=0.1. This is based on a midpoint calculation. Otherwise based on edge computing. The damage is 45/50=0.9.
    /// 以圆形伤害区域为例：半径为50,生物到中点的距离为45,那么收到的伤害为45/50=0.9,1-0.9=0.1。这是基于中点计算的结果。否则基于边缘计算。伤害为45/50=0.9。
    /// </remarks>
    private bool _isDamageCenterBased = true;

    public override void _Ready()
    {
        base._Ready();
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        InputPickable = false;
        SetCollisionMaskValue(Config.LayerNumber.Player, true);
        SetCollisionMaskValue(Config.LayerNumber.Mob, true);
        Monitoring = true;
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
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
        LogCat.Log("BodyExited");
        if (body is CharacterTemplate characterTemplate)
        {
            _characterTemplates.Remove(characterTemplate);
        }
    }

    /// <summary>
    /// <para>When a player or creature exits the damage area</para>
    /// <para>当玩家或生物退出伤害区域</para>
    /// </summary>
    /// <param name="body"></param>
    private void OnBodyEntered(Node2D body)
    {
        LogCat.Log("BodyEntered");
        if (body is CharacterTemplate characterTemplate)
        {
            _characterTemplates.Add(characterTemplate);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_collisionShape2D == null || _rangeDamage == null)
        {
            return;
        }
        if (_collisionShape2D.Shape is CircleShape2D circleShape2D)
        {
            //CircleShape2D
            //圆形
            foreach (var characterTemplate in _characterTemplates)
            {
                var distance = characterTemplate.GlobalPosition.DistanceTo(_collisionShape2D.GlobalPosition);
                if (distance > circleShape2D.Radius)
                {
                    //The creature or player is outside the shape.
                    //生物或玩家在形状外围。
                    continue;
                }
                float percent;
                if (_isDamageCenterBased)
                {
                    percent = 1 - distance / circleShape2D.Radius;
                }
                else
                {
                    percent = distance / circleShape2D.Radius;
                }
                characterTemplate.Damage(new FixedDamage(_rangeDamage.MinDamage + (int)(_rangeDamage.MaxDamage - _rangeDamage.MinDamage * percent)));
            }
        }
        else if (_collisionShape2D.Shape is RectangleShape2D rectangleShape2D)
        {
            //Rectangular shape 2D
            //矩形形状2D
            foreach (var characterTemplate in _characterTemplates)
            {
                var rectangleShapeRect = rectangleShape2D.GetRect();
                var rect = new Rect2(new Vector2(rectangleShapeRect.Position.X + _collisionShape2D.GlobalPosition.X, rectangleShapeRect.Position.Y + _collisionShape2D.GlobalPosition.Y), rectangleShapeRect.Size);
                //Determines whether a coordinate is contained within the rectangle.
                //判断某个坐标是否包含在矩形内。
                if (rect.HasPoint(characterTemplate.GlobalPosition))
                {
                    //The coordinates of the creature are inside the rectangle.
                    //生物的坐标在矩形内。
                    characterTemplate.Damage(new FixedDamage(_rangeDamage.MinDamage));
                }
            }
        }
        else
        {
            LogCat.LogError("invalid_damage_shape");
        }
    }
}