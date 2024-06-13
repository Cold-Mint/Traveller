using System.Collections.Generic;

using ColdMint.scripts.debug;
using ColdMint.scripts.projectile;
using ColdMint.scripts.utils;

using Godot;

namespace ColdMint.scripts.item.weapon;

/// <summary>
/// <para>Projectile weapons</para>
/// <para>抛射体武器</para>
/// </summary>
/// <remarks>
///<para>These weapons can fire projectiles to attack the enemy.For example: guns and scepters.Generate a bullet to attack the enemy.</para>
///<para>这类武器可发射抛射体，攻击敌人。例如：枪支和法杖。生成一个子弹攻击敌人。</para>
/// </remarks>
public partial class ProjectileWeapon : WeaponTemplate
{
    /// <summary>
    /// <para>The formation position of the projectile</para>
    /// <para>抛射体的生成位置</para>
    /// </summary>
    private Marker2D? _marker2D;

    // /// <summary>
    // /// <para>List of projectiles</para>
    // /// <para>抛射体列表</para>
    // /// </summary>
    // private string[]? _projectiles;
    //
    // private Dictionary<string, PackedScene>? _projectileCache;

    [Export] protected PackedScene[] ProjectileScenes { get; set; } = [];

    private Node2D? _projectileContainer;

    public override void _Ready()
    {
        base._Ready();
        _marker2D = GetNode<Marker2D>("Marker2D");

        // _projectileCache = new Dictionary<string, PackedScene>();
        // _projectiles = GetMeta("Projectiles", "").AsStringArray();
        // foreach (var projectileItem in _projectiles)
        // {
        //     var packedScene = GD.Load<PackedScene>(projectileItem);
        //     if (packedScene == null)
        //     {
        //         continue;
        //     }
        //
        //     _projectileCache.Add(projectileItem, packedScene);
        // }

        _projectileContainer = GetNode("/root/Game/ProjectileContainer") as Node2D;
    }


    protected override void DoFire(Node2D? owner, Vector2 enemyGlobalPosition)
    {
        if (owner == null || _projectileContainer == null || _marker2D == null) return;

        //空列表检查
        if (ProjectileScenes is [])
        {
            LogCat.LogError("projectiles_is_empty");
            return;
        }

        //Get the first projectile
        //获取第一个抛射体
        var projectileScene = ProjectileScenes[0];
        // var projectileScene = _projectileCache[_projectiles[0]];
        var projectile = NodeUtils.InstantiatePackedScene<ProjectileTemplate>(projectileScene, _projectileContainer);
        if (projectile == null) return;
        projectile.Owner = owner;
        projectile.Velocity = (enemyGlobalPosition - _marker2D.GlobalPosition).Normalized() * projectile.Speed;
        projectile.Position = _marker2D.GlobalPosition;
    }
}