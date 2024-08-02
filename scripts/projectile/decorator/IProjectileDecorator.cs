namespace ColdMint.scripts.projectile.decorator;

/// <summary>
/// <para>Projectile decorator</para>
/// <para>抛射体装饰</para>
/// </summary>
/// <remarks>
///<para>Decorator mode is a structural mode, which can add special features to the projectile.</para>
///<para>装饰模式是一种结构性模式，可以将一种抛射体的特殊功能添加到抛射体上。</para>
/// </remarks>
public interface IProjectileDecorator
{
    /// <summary>
    /// <para>When the character is killed by this projectile</para>
    /// <para>当角色被此抛射体击杀时</para>
    /// </summary>
    void OnKillCharacter();
}