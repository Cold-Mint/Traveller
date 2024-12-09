using ColdMint.scripts.projectile;
using ColdMint.scripts.projectile.decorator;
using Godot;

namespace ColdMint.scripts.spell;

/// <summary>
/// <para>Generate a set node spell after killing the character</para>
/// <para>杀死角色后生成制定节点法术</para>
/// </summary>
public partial class NodeSpawnOnKillCharacterSpell : SpellPickAble
{
    [Export]
    private string? _packedScenePath; // skipcq:CS-R1137
    private NodeSpawnOnKillCharacterDecorator? _nodeSpawnOnKillCharacterDecorator;

    public override void LoadResource()
    {
        base.LoadResource();
        if (_nodeSpawnOnKillCharacterDecorator == null && !string.IsNullOrEmpty(_packedScenePath))
        {
            _nodeSpawnOnKillCharacterDecorator = new NodeSpawnOnKillCharacterDecorator
            {
                PackedScenePath = _packedScenePath,
                DefaultParentNode = this
            };
        }
    }

    public override void ModifyProjectile(int index, Projectile projectile, ref Vector2 velocity)
    {
        if (_nodeSpawnOnKillCharacterDecorator == null)
        {
            return;
        }
        projectile.AddProjectileDecorator(_nodeSpawnOnKillCharacterDecorator);
    }
}