using ColdMint.scripts.debug;
using ColdMint.scripts.projectile;
using ColdMint.scripts.projectile.decorator;
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

    /// <summary>
    /// <para>Whether to launch in the order of the projectile list</para>
    /// <para>是否按照抛射体列表的循序发射</para>
    /// </summary>
    [Export]
    protected bool Sequentially { get; set; }

    private int _projectileIndex;

    public override void _Ready()
    {
        base._Ready();
        _marker2D = GetNode<Marker2D>("Marker2D");
    }

    /// <summary>
    /// <para>GetNextProjectileScene</para>
    /// <para>获取下一个抛射体</para>
    /// </summary>
    /// <returns></returns>
    private PackedScene GetNextProjectileScene()
    {
        if (Sequentially)
        {
            _projectileIndex = (_projectileIndex + 1) % ProjectileScenes.Length;
            return ProjectileScenes[_projectileIndex];
        }
        else
        {
            return ProjectileScenes[RandomUtils.Instance.Next(ProjectileScenes.Length)];
        }
    }


    protected override void DoFire(Node2D? owner, Vector2 enemyGlobalPosition)
    {
        if (owner == null)
        {
            LogCat.LogError("owner_is_null");
            return;
        }

        if (_marker2D == null)
        {
            LogCat.LogError("marker2d_is_null");
            return;
        }

        if (GameSceneNodeHolder.ProjectileContainer == null)
        {
            LogCat.LogError("projectile_container_is_null");
            return;
        }

        //Empty list check
        //空列表检查
        if (ProjectileScenes is [])
        {
            LogCat.LogError("projectiles_is_empty");
            return;
        }

        //Get the first projectile
        //获取第一个抛射体
        var projectileScene = GetNextProjectileScene();
        var projectile = NodeUtils.InstantiatePackedScene<Projectile>(projectileScene);
        if (projectile == null) return;
        if (Config.IsDebug())
        {
            var nodeSpawnOnKillCharacterDecorator = new NodeSpawnOnKillCharacterDecorator
            {
                DefaultParentNode = this,
                PackedScenePath = "res://prefab/entitys/BlackenedAboriginalWarrior.tscn"
            };
            projectile.AddProjectileDecorator(nodeSpawnOnKillCharacterDecorator);
        }

        NodeUtils.CallDeferredAddChild(GameSceneNodeHolder.ProjectileContainer, projectile);
        projectile.Owner = owner;
        projectile.Velocity = (enemyGlobalPosition - _marker2D.GlobalPosition).Normalized() * projectile.Speed;
        projectile.Position = _marker2D.GlobalPosition;
    }
}