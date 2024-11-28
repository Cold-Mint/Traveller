using ColdMint.scripts.character;
using ColdMint.scripts.damage;
using ColdMint.scripts.debug;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.projectile.decorator;

/// <summary>
/// <para>DamageAreaDecorator</para>
/// <para>伤害区域装饰器</para>
/// </summary>
public class DamageAreaDecorator : IProjectileDecorator
{
    private string? _packedScenePath;
    public string? PackedScenePath
    {
        get => _packedScenePath;
        set
        {
            _packedScenePath = value;
            LoadPackScene();
        }
    }
    private PackedScene? _packedScene;
    public RangeDamage? RangeDamage { get; set; }

    /// <summary>
    /// <para>LoadPackScene</para>
    /// <para>加载打包的场景</para>
    /// </summary>
    private void LoadPackScene()
    {
        if (string.IsNullOrEmpty(_packedScenePath))
        {
            return;
        }
        _packedScene = ResourceLoader.Load<PackedScene>(_packedScenePath);
    }

    public void OnKillCharacter(Node2D? owner, CharacterTemplate target)
    {

    }

    public void Attach(Projectile projectile)
    {
    }

    public void Detach(Projectile projectile)
    {
    }

    public bool SupportedModificationPhysicalFrame
    {
        get => true;
    }

    public void PhysicsProcess(Projectile projectile, KinematicCollision2D? collisionInfo)
    {
        if (collisionInfo == null || _packedScene == null || RangeDamage == null || GameSceneDepend.DynamicDamageAreaContainer == null)
        {
            // LogCat.Log("collisionInfo " + (collisionInfo == null) + " || _packedScene" + (_packedScene == null) + " || RangeDamage" + (RangeDamage == null) + " || GameSceneDepend.DynamicDamageAreaContainer " + (GameSceneDepend.DynamicDamageAreaContainer == null));
            return;
        }
        var damageArea = NodeUtils.InstantiatePackedScene<DamageArea>(_packedScene);
        if (damageArea == null)
        {
            return;
        }
        //TODO：更新伤害区域的列表，以便其能够正确的计算伤害。
        damageArea.OwnerNode = projectile.OwnerNode;
        damageArea.SetDamage(RangeDamage);
        damageArea.GlobalPosition = ((Node2D)collisionInfo.GetCollider()).GlobalPosition;
        damageArea.OneShot = true;
        damageArea.AddResidualUse(1);
        NodeUtils.CallDeferredAddChild(GameSceneDepend.DynamicDamageAreaContainer, damageArea);
    }
}