using ColdMint.scripts.pickable;
using ColdMint.scripts.projectile;
using ColdMint.scripts.weapon;
using Godot;

namespace ColdMint.scripts.spell;

/// <summary>
/// <para>magic</para>
/// <para>法术</para>
/// </summary>
/// <remarks>
///<para>For projectile weapons</para>
///<para>用于抛射体武器</para>
/// </remarks>
public partial class SpellPickAble : PickAbleTemplate, ISpell
{
    [Export]
    private string? _projectilePath;

    /// <summary>
    /// <para>Spells do no collision damage</para>
    /// <para>法术没有碰撞伤害</para>
    /// </summary>
    public override bool EnableContactInjury
    {
        get => false;
        set { }
    }

    private PackedScene? _projectileScene;
    public override void _Ready()
    {
        base._Ready();
        if (_projectilePath != null)
        {
            _projectileScene = GD.Load<PackedScene>(_projectilePath);
        }
    }

    public override int ItemType
    {
        get => Config.ItemType.Spell;
    }

    public PackedScene? GetProjectile()
    {
        return _projectileScene;
    }

    public virtual void ModifyWeapon(ProjectileWeapon projectileWeapon)
    {

    }

    public virtual void RestoreWeapon(ProjectileWeapon projectileWeapon)
    {

    }

    public virtual void ModifyProjectile(int index, Projectile projectile, ref Vector2 velocity)
    {

    }
}