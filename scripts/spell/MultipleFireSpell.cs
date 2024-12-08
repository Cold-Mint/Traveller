using ColdMint.scripts.projectile;
using ColdMint.scripts.utils;
using ColdMint.scripts.weapon;
using Godot;

namespace ColdMint.scripts.spell;

/// <summary>
/// <para>MultipleFireSpell</para>
/// <para>多重射击法术</para>
/// </summary>
/// <remarks>
///<para>Use this spell to create shotgun effects</para>
///<para>通过此法术打造霰弹枪的效果</para>
/// </remarks>
public partial class MultipleFireSpell : SpellPickAble
{
    /// <summary>
    /// <para>How many projectiles are generated per fire</para>
    /// <para>每次开火生成多少个抛射体</para>
    /// </summary>
    [Export]
    public int NumberOfProjectiles { get; set; } = 3;

    /// <summary>
    /// <para>RandomAngle</para>
    /// <para>随机角度</para>
    /// </summary>
    [Export]
    public bool RandomAngle { get; set; }

    /// <summary>
    /// <para>Unit radian</para>
    /// <para>单位弧度</para>
    /// </summary>
    /// <remarks>
    ///<para>Unit radian of correction for the projectile Angle.Suppose there are three bullets fired at once, and this is the arc between the two bullets.</para>
    ///<para>对抛射体角度修正的单位弧度。假定有三颗子弹一次发射，这是两颗子弹之间的弧度。</para>
    /// </remarks>
    [Export]
    public float UnitRadian { get; set; } = 0.069813f;

    /// <summary>
    /// <para>initial Radian</para>
    /// <para>起始弧度</para>
    /// </summary>
    ///<remarks>
    ///<para>The Angle of the first bullet, and subsequent bullets will be offset in unit radians.</para>
    ///<para>第一颗子弹的角度，随后的子弹会以单位弧度偏移。</para>
    /// </remarks>
    private float _initialRadian;
    private float _maxRadian;
    private int _oldNumberOfProjectiles;
    public override void ModifyWeapon(ProjectileWeapon projectileWeapon)
    {
        base.ModifyWeapon(projectileWeapon);
        _oldNumberOfProjectiles = projectileWeapon.NumberOfProjectiles;
        projectileWeapon.NumberOfProjectiles = NumberOfProjectiles;
        _initialRadian = -(NumberOfProjectiles / 2f * UnitRadian);
        _maxRadian = NumberOfProjectiles * UnitRadian;
    }

    public override void RestoreWeapon(ProjectileWeapon projectileWeapon)
    {
        base.RestoreWeapon(projectileWeapon);
        projectileWeapon.NumberOfProjectiles = _oldNumberOfProjectiles;
    }

    public override void ModifyProjectile(int index, Projectile projectile, ref Vector2 velocity)
    {
        base.ModifyProjectile(index, projectile, ref velocity);
        velocity = RandomAngle ? velocity.Rotated(_initialRadian + _maxRadian * RandomUtils.Instance.NextSingle()) : velocity.Rotated(_initialRadian + UnitRadian * index);
    }

}