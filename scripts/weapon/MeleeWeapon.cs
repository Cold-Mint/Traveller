using ColdMint.scripts.damage;
using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.weapon;

/// <summary>
/// <para>MeleeWeapon</para>
/// <para>近战武器</para>
/// </summary>
public partial class MeleeWeapon : WeaponTemplate
{

    private DamageArea? _damageArea;
    [Export]
    private int _minDamage; // skipcq:CS-R1137

    [Export]
    private int _maxDamage; // skipcq:CS-R1137
    [Export]
    private Config.DamageType _damageType = Config.DamageType.Physical; // skipcq:CS-R1137
    [Export]
    private int _criticalStrikeProbability; // skipcq:CS-R1137
    public override void LoadResource()
    {
        base.LoadResource();
        _damageArea = GetNode<DamageArea>("WeaponDamageArea");
        _damageArea.OwnerNode = OwnerNode;
        _damageArea.SetDamage(new RangeDamage
        {
            MinDamage = _minDamage,
            MaxDamage = _maxDamage,
            Type = _damageType,
            CriticalStrikeProbability = _criticalStrikeProbability
        });
    }

    protected override void OnOwnerNodeChanged(Node2D? node2D)
    {
        base.OnOwnerNodeChanged(node2D);
        if (_damageArea != null)
        {
            _damageArea.OwnerNode = node2D;
        }
    }

    protected override bool DoFire(Node2D? owner, Vector2 enemyGlobalPosition)
    {
        if (owner == null)
        {
            LogCat.LogError("owner_is_null", LogCat.LogLabel.MeleeWeapon);
            return false;
        }
        if (_damageArea == null)
        {
            return false;
        }
        _damageArea.AddResidualUse(1);
        return true;
    }
}