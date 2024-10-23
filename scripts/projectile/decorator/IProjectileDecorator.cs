using ColdMint.scripts.character;
using Godot;

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
    /// <param name="owner">
    /// <para>owner</para>
    /// <para>主人</para>
    /// </param>
    /// <param name="target">
    ///<para>target</para>
    ///<para>目标</para>
    /// </param>
    void OnKillCharacter(Node2D? owner, CharacterTemplate target);

    /// <summary>
    /// <para>When a decorator is attached to a projectile (note that the same decorator can be attached to multiple projectiles.)</para>
    /// <para>当装饰器附加到抛射体上时（注意同一个装饰器可附加到多个抛射体上。）</para>
    /// </summary>
    /// <param name="projectile"></param>
    void Attach(Projectile projectile);

    /// <summary>
    /// <para>When the decorator is removed from the projectile</para>
    /// <para>当装饰器从抛射体上移除时</para>
    /// </summary>
    /// <param name="projectile"></param>
    void Detach(Projectile projectile);

    /// <summary>
    /// <para>SupportedModificationPhysicalFrame</para>
    /// <para>支持修改物理帧吗</para>
    /// </summary>
    bool SupportedModificationPhysicalFrame { get; }

    /// <summary>
    /// <para>Each physical frame is processed</para>
    /// <para>处理每个物理帧</para>
    /// </summary>
    /// <param name="projectile">
    ///<para>The current physical frame for which projectile?</para>
    ///<para>目前为哪个抛射体的物理帧？</para>
    /// </param>
    /// <param name="collisionInfo">
    ///<para>collisionInfo</para>
    ///<para>碰撞信息</para>
    /// </param>
    void PhysicsProcess(Projectile projectile, KinematicCollision2D? collisionInfo);
}