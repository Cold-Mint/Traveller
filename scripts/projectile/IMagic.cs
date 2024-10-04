using ColdMint.scripts.weapon;
using Godot;

namespace ColdMint.scripts.projectile;

/// <summary>
/// <para>Magic</para>
/// <para>法术</para>
/// </summary>
/// <remarks>
///<para>For projectile weapons</para>
///<para>用于抛射体武器</para>
/// </remarks>
public interface IMagic
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
    /// <param name="projectile"></param>
    void ModifyProjectile(Projectile projectile);

}