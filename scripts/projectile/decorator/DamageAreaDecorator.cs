using System.Collections.Generic;
using ColdMint.scripts.character;
using ColdMint.scripts.damage;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.projectile.decorator;

/// <summary>
/// <para>DamageAreaDecorator</para>
/// <para>伤害区域装饰器</para>
/// </summary>
public class DamageAreaDecorator : IProjectileDecorator
{
    public string? PackedScenePath { get; set; }
    public RangeDamage? RangeDamage { get; set; }
    private readonly Dictionary<Projectile, DamageArea> _damageAreaCache = new();


    public void OnKillCharacter(Node2D? owner, CharacterTemplate target)
    {

    }

    public void Attach(Projectile projectile)
    {
        if (PackedScenePath == null || RangeDamage == null)
        {
            return;
        }
        var packedScene = ResourceLoader.Load<PackedScene>(PackedScenePath);
        if (packedScene == null)
        {
            return;
        }
        var damageArea = NodeUtils.InstantiatePackedScene<DamageArea>(packedScene);
        if (damageArea == null)
        {
            return;
        }
        damageArea.SetDamage(RangeDamage);
        NodeUtils.CallDeferredAddChild(projectile, damageArea);
        _damageAreaCache.Add(projectile, damageArea);
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
        get => false;
    }

    public void PhysicsProcess(Projectile projectile, KinematicCollision2D? collisionInfo)
    {
        if (collisionInfo == null)
        {
            return;
        }
        if (_damageAreaCache.TryGetValue(projectile, out var damageArea))
        {
            damageArea.AddResidualUse(1);
        }
    }
}