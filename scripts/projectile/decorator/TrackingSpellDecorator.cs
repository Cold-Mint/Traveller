using System.Collections.Generic;
using ColdMint.scripts.character;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.projectile.decorator;

/// <summary>
/// <para>Track spell decorator</para>
/// <para>跟踪法术装饰器</para>
/// </summary>
/// <remarks>
///<para>Provides the projectile with the ability to track enemies.</para>
///<para>为抛射体提供跟踪敌人的能力。</para>
/// </remarks>
public class TrackingSpellDecorator : IProjectileDecorator
{
    /// <summary>
    /// <para>Rays used to detect walls</para>
    /// <para>用于检测墙壁的射线</para>
    /// </summary>
    private readonly Dictionary<Projectile, RayCast2D> _walRaycastCache = new();


    /// <summary>
    /// <para>Target node to track</para>
    /// <para>要跟踪的目标节点</para>
    /// </summary>
    public Node2D? TargetNode { get; set; }

    public void OnKillCharacter(Node2D? owner, CharacterTemplate target)
    {

    }

    public void Attach(Projectile projectile)
    {
        if (!projectile.IgnoreWall)
        {
            var rayCast2D = new RayCast2D();
            rayCast2D.SetCollisionMaskValue(Config.LayerNumber.Wall, true);
            rayCast2D.SetCollisionMaskValue(Config.LayerNumber.Floor, true);
            NodeUtils.CallDeferredAddChild(projectile, rayCast2D);
            _walRaycastCache.Add(projectile, rayCast2D);
        }
        if (TargetNode != null)
        {
            TargetNode.TreeExiting += () =>
            {
                TargetNode = null;
            };
        }
    }


    public void Detach(Projectile projectile)
    {
        if (_walRaycastCache.ContainsKey(projectile))
        {
            _walRaycastCache.Remove(projectile);
        }
    }
    public bool SupportedModificationPhysicalFrame
    {
        get => true;
    }

    public void PhysicsProcess(Projectile projectile, KinematicCollision2D? collisionInfo)
    {
        //If no object is hit (collisionInfo == null) and there is a tracking target.
        //如果没有撞到任何对象(collisionInfo == null)且有跟踪的目标。
        if (collisionInfo == null && TargetNode != null)
        {
            //Gets a vector of the projectile pointing at the enemy's position.
            //得到抛射体指向敌人位置的向量。
            var desiredVelocity = TargetNode.GlobalPosition - projectile.GlobalPosition;
            if (_walRaycastCache.TryGetValue(projectile, out var rayCast2D))
            {
                rayCast2D.TargetPosition = desiredVelocity.Rotated(-projectile.Rotation);
                if (rayCast2D.IsColliding())
                {
                    return;
                }
            }
            var actualDesiredVelocity = desiredVelocity.Normalized() * projectile.ActualSpeed;
            //The weight is smaller, the circle is larger.
            //weight越小，子弹绕的圈越大。
            projectile.Velocity = projectile.Velocity.Lerp(actualDesiredVelocity, 0.1f);
        }
    }

}