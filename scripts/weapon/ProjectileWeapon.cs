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

    /// <summary>
    /// <para>Scattering radians</para>
    /// <para>散射弧度</para>
    /// </summary>
    [Export] protected float OffsetAngle;

    /// <summary>
    /// <para>Offset angle mode</para>
    /// <para>偏移角度模式</para>
    /// </summary>
    [Export] protected int OffsetAngleMode = Config.OffsetAngleMode.Random;

    /// <summary>
    /// <para>Whether the last offset angle is positive</para>
    /// <para>上次的偏移角度是否为正向的</para>
    /// </summary>
    private bool _positiveOffsetAngle = true;

    /// <summary>
    /// <para>The number of projectiles fired at once</para>
    /// <para>一次可以发射多少个子弹</para>
    /// </summary>
    [Export] protected float NumberOfProjectiles = 1;

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


    /// <summary>
    /// <para>GetRandomAngle</para>
    /// <para>获取随机的偏移弧度</para>
    /// </summary>
    /// <returns></returns>
    private float GetRandomAngle()
    {
        if (OffsetAngle == 0)
        {
            //If the offset angle is 0, then return 0
            //弧度为0,不用偏移。
            return 0;
        }

        if (OffsetAngleMode == Config.OffsetAngleMode.Cross)
        {
            float result;
            if (_positiveOffsetAngle)
            {
                result = -OffsetAngle / 2;
            }
            else
            {
                result = OffsetAngle / 2;
            }

            _positiveOffsetAngle = !_positiveOffsetAngle;
            return result;
        }

        if (OffsetAngleMode == Config.OffsetAngleMode.AlwaysSame)
        {
            return OffsetAngle;
        }

        var min = -OffsetAngle / 2;
        return min + RandomUtils.Instance.NextSingle() * OffsetAngle;
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
        for (int i = 0; i < NumberOfProjectiles; i++)
        {
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
            projectile.TargetNode = GameSceneNodeHolder.TemporaryTargetNode;
            projectile.Velocity =
                (_marker2D.GlobalPosition.DirectionTo(enemyGlobalPosition) * projectile.Speed)
                .Rotated(GetRandomAngle());
            projectile.Position = _marker2D.GlobalPosition;
        }
    }
}