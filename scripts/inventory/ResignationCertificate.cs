using ColdMint.scripts.character;
using ColdMint.scripts.damage;
using ColdMint.scripts.weapon;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>ResignationCertificate</para>
/// <para>离职证明</para>
/// </summary>
public partial class ResignationCertificate : WeaponTemplate
{
    private readonly Damage _damage = new()
    {
        MaxDamage = 1,
        MinDamage = 1,
        Type = Config.DamageType.Magic
    };

    public override void _Ready()
    {
        base._Ready();
        _damage.CreateDamage();
        _damage.Attacker = this;
    }

    public override int ItemType
    {
        get => Config.ItemType.Item;
    }
    protected override bool DoFire(Node2D? owner, Vector2 enemyGlobalPosition)
    {
        if (Owner is CharacterTemplate characterTemplate)
        {
            return characterTemplate.Damage(_damage);
        }
        return false;
    }
}