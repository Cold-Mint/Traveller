using System.Collections.Generic;
using ColdMint.scripts.character;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.projectile.decorator;

/// <summary>
/// <para>Damage area spell</para>
/// <para>伤害范围法术</para>
/// </summary>
public class DamageRangeDecorator : IProjectileDecorator
{
    private readonly Dictionary<Projectile, Area2D> _damageAreaCache = new();
    public PackedScene? DamageAreaPackedScene { get; set; }

    public void OnKillCharacter(Node2D? owner, CharacterTemplate target)
    {

    }

    public void Attach(Projectile projectile)
    {
        var damageArea2D = CreateDamageArea();
        if (damageArea2D == null)
        {
            return;
        }
        NodeUtils.CallDeferredAddChild(projectile, damageArea2D);
        _damageAreaCache.Add(projectile, damageArea2D);
    }

    /// <summary>
    /// <para>CreateDamageArea</para>
    /// <para>创建伤害区域</para>
    /// </summary>
    /// <returns></returns>
    private Area2D? CreateDamageArea()
    {
        if (DamageAreaPackedScene == null)
        {
            return null;
        }
        return NodeUtils.InstantiatePackedScene<Area2D>(DamageAreaPackedScene);
    }

    public void Detach(Projectile projectile)
    {
        if (_damageAreaCache.TryGetValue(projectile, out var area2D))
        {
            area2D.QueueFree();
            _damageAreaCache.Remove(projectile);
        }
    }

    public bool SupportedModificationPhysicalFrame
    {
        get => true;
    }

    public void PhysicsProcess(Projectile projectile, KinematicCollision2D? collisionInfo)
    {
        if (collisionInfo == null)
        {
            //Didn't hit anything.
            //没有撞到任何对象。
            return;
        }
        //Activate the damage area when you bump into any object.
        //当撞到任何对象时，都要激活伤害区域。
        if (!_damageAreaCache.TryGetValue(projectile, out var area2D))
        {
            return;
        }
        area2D.Monitoring = true;
    }

}