using ColdMint.scripts.damage;
using ColdMint.scripts.projectile;
using ColdMint.scripts.projectile.decorator;
using Godot;

namespace ColdMint.scripts.spell;

/// <summary>
/// <para>Area damage spell</para>
/// <para>范围伤害法术</para>
/// </summary>
public partial class DamageAreaSell : SpellPickAble
{

    [Export]
    public string? PackedScenePath { get; set; }

    private DamageAreaDecorator? _damageAreaDecorator;

    [Export]
    private int MinDamage { get; set; }
    [Export]
    private int MaxDamage { get; set; }
    [Export]
    private Config.DamageType _damageType = Config.DamageType.Physical; // skipcq:CS-R1137

    public override void LoadResource()
    {
        base.LoadResource();
        if (_damageAreaDecorator == null)
        {
            _damageAreaDecorator = new DamageAreaDecorator
            {
                RangeDamage = new RangeDamage
                {
                    MaxDamage = MaxDamage,
                    MinDamage = MinDamage,
                    Type = _damageType,
                    Attacker = OwnerNode
                },
                PackedScenePath = PackedScenePath
            };
        }
    }

    public override void ModifyProjectile(int index, Projectile projectile, ref Vector2 velocity)
    {
        if (_damageAreaDecorator == null)
        {
            return;
        }
        projectile.AddProjectileDecorator(_damageAreaDecorator);
    }
}