using ColdMint.scripts.debug;
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
    private string? _packedScenePath;
    private NodeSpawnOnKillCharacterDecorator? _nodeSpawnOnKillCharacterDecorator;
    public override void _Ready()
    {
        base._Ready();
        if (!string.IsNullOrEmpty(_packedScenePath))
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
            LogCat.Log("添加失败");
            return;
        }
        LogCat.Log("添加成功");
        projectile.AddProjectileDecorator(_nodeSpawnOnKillCharacterDecorator);
    }
}