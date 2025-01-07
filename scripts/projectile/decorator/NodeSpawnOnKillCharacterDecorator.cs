using ColdMint.scripts.character;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.projectile.decorator;

/// <summary>
/// <para>NodeSpawnOnKillCharacterDecorator</para>
/// <para>在击杀角色后生成节点</para>
/// </summary>
public class NodeSpawnOnKillCharacterDecorator : IProjectileDecorator
{
    private string? _packedScenePath;

    /// <summary>
    /// <para>PackedScenePath</para>
    /// <para>要实例化的场景路径</para>
    /// </summary>
    public string? PackedScenePath
    {
        get => _packedScenePath;
        set
        {
            _packedScenePath = value;
            LoadPackScene();
        }
    }

    /// <summary>
    /// <para>DefaultParentNode</para>
    /// <para>默认的父节点</para>
    /// </summary>
    public Node? DefaultParentNode { get; set; }

    /// <summary>
    /// <para>Chance</para>
    /// <para>生成概率</para>
    /// </summary>
    public float Chance { get; set; } = 1f;

    private PackedScene? _packedScene;

    /// <summary>
    /// <para>LoadPackScene</para>
    /// <para>加载打包的场景</para>
    /// </summary>
    private void LoadPackScene()
    {
        if (string.IsNullOrEmpty(PackedScenePath))
        {
            return;
        }
        _packedScene = ResourceLoader.Load<PackedScene>(PackedScenePath);
    }

    public void OnKillCharacter(Node2D? owner, CharacterTemplate target)
    {
        if (GD.Randf() > Chance)
        {
            //Not in probability, straight back.
            //没有在概率内，直接返回。
            return;
        }

        if (_packedScene == null || DefaultParentNode == null || !target.CanMutateAfterDeath)
        {
            return;
        }
        var node2D = NodeUtils.InstantiatePackedScene<Node2D>(_packedScene);
        if (node2D == null)
        {
            return;
        }

        var container = NodeUtils.FindContainerNode(node2D, DefaultParentNode);
        node2D.GlobalPosition = target.GlobalPosition;
        NodeUtils.CallDeferredAddChild(container, node2D);
    }

    public void Attach(Projectile projectile)
    {

    }

    public void Detach(Projectile projectile)
    {

    }
    public bool SupportedModificationPhysicalFrame
    {
        get => false;
    }

    public void PhysicsProcess(Projectile projectile, KinematicCollision2D? collisionInfo)
    {

    }
}