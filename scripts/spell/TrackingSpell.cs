using ColdMint.scripts.projectile;
using Godot;

namespace ColdMint.scripts.spell;

public partial class TrackingSpell : SpellPickAble
{
    public override void ModifyProjectile(int index, Projectile projectile, ref Vector2 velocity)
    {
        base.ModifyProjectile(index, projectile, ref velocity);
        projectile.EnableTracking = true;
    }
}