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

    private PackedScene? _projectileScene;
    public sealed override void _Ready()
    {
        base._Ready();
        LoadResource();
    }

    public override int ItemType
    {
        get => Config.ItemType.Spell;
    }

    public PackedScene? GetProjectile()
    {
        return _projectileScene;
    }

    public virtual void LoadResource()
    {
        if (_projectileScene == null && !string.IsNullOrEmpty(_projectilePath))
        {
            _projectileScene = ResourceLoader.Load<PackedScene>(_projectilePath);
        }
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