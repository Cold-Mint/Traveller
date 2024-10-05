using ColdMint.scripts.weapon;
using Godot;

namespace ColdMint.scripts.projectile;

/// <summary>
/// <para>Spell</para>
/// <para>法术</para>
/// </summary>
/// <remarks>
///<para>For projectile weapons</para>
///<para>用于抛射体武器</para>
/// </remarks>
public interface ISpell
{
    /// <summary>
    /// <para>GetProjectile</para>
    /// <para>获取抛射体</para>
    /// </summary>
    /// <returns></returns>
    PackedScene? GetProjectile();

    /// <summary>
    /// <para>Modify Weapon</para>
    /// <para>修改武器</para>
    /// </summary>
    /// <param name="projectileWeapon"></param>
    void ModifyWeapon(ProjectileWeapon projectileWeapon);

    /// <summary>
    /// <para>Restores the modified weapon properties</para>
    /// <para>还原修改的武器属性</para>
    /// </summary>
    /// <param name="projectileWeapon"></param>
    void RestoreWeapon(ProjectileWeapon projectileWeapon);

    /// <summary>
    /// <para>Modify the projectile</para>
    /// <para>修改抛射体</para>
    /// </summary>
    /// <param name="index">
    ///<para>What is the current projectile? For example, a weapon can fire three projectiles at once, with indexes 0,1,2</para>
    ///<para>当前抛射体是第几个？例如：武器可一下发射3个抛射体，索引为0,1,2</para>
    /// </param>
    /// <param name="projectile">
    ///<para>Projectile object</para>
    ///<para>抛射体对象</para>
    /// </param>
    /// <param name="velocity">
    ///<para>The velocity of the projectile</para>
    ///<para>抛射体的飞行速度</para>
    /// </param>
    void ModifyProjectile(int index,Projectile projectile, ref Vector2 velocity);

}