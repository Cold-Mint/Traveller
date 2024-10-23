using ColdMint.scripts.projectile;
using ColdMint.scripts.projectile.decorator;
using ColdMint.scripts.weapon;
using Godot;

namespace ColdMint.scripts.spell;

/// <summary>
/// <para>TrackingSpell</para>
/// <para>跟踪法术</para>
/// </summary>
public partial class TrackingSpell : SpellPickAble
{

    private TrackingSpellDecorator? _trackingSpellDecorator;

    public override void ModifyWeapon(ProjectileWeapon projectileWeapon)
    {
        base.ModifyWeapon(projectileWeapon);
        if (GameSceneDepend.TemporaryTargetNode == null)
        {
            return;
        }
        //Create a new decorator for each fire, instead of referencing a member variable. This allows a target to be tracked each time a fire is fired, and does not allow the target to be changed again after firing.
        //为每次开火创建一个新的装饰器，而不是引用成员变量。这使每次开火都会去跟踪一个目标，不允许发射后再次改变目标。
        _trackingSpellDecorator = new TrackingSpellDecorator
        {
            TargetNode = GameSceneDepend.TemporaryTargetNode,
        };
    }

    public override void RestoreWeapon(ProjectileWeapon projectileWeapon)
    {
        base.RestoreWeapon(projectileWeapon);
        _trackingSpellDecorator = null;
    }

    public override void ModifyProjectile(int index, Projectile projectile, ref Vector2 velocity)
    {
        base.ModifyProjectile(index, projectile, ref velocity);
        if (_trackingSpellDecorator == null)
        {
            return;
        }
        projectile.AddProjectileDecorator(_trackingSpellDecorator);
    }
}