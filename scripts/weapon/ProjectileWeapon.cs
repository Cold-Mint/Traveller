using ColdMint.scripts.debug;
using ColdMint.scripts.projectile;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.weapon;

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

    [Export] protected PackedScene[] ProjectileScenes { get; set; } = [];

    private Node2D? _projectileContainer;

    public override void _Ready()
    {
        base._Ready();
        _marker2D = GetNode<Marker2D>("Marker2D");

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
        var projectile = NodeUtils.InstantiatePackedScene<ProjectileTemplate>(projectileScene);
        if (projectile == null) return;
        NodeUtils.CallDeferredAddChild(_projectileContainer, projectile);
        projectile.Owner = owner;
        projectile.Velocity = (enemyGlobalPosition - _marker2D.GlobalPosition).Normalized() * projectile.Speed;
        projectile.Position = _marker2D.GlobalPosition;
    }
}