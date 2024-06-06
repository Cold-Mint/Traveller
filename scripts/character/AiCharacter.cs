using System;
using System.Collections.Generic;
using ColdMint.scripts.behaviorTree;
using Godot;

namespace ColdMint.scripts.character;

/// <summary>
/// <para>The role played by computers</para>
/// <para>由电脑扮演的角色</para>
/// </summary>
public sealed partial class AiCharacter : CharacterTemplate
{
    /// <summary>
    /// <para>How fast the character moves</para>
    /// <para>角色的移动速度</para>
    /// </summary>
    private float _movementSpeed = 300.0f;

    private BehaviorNode? _behaviorNode;

    //Used to detect rays on walls
    //用于检测墙壁的射线
    private RayCast2D? _wallDetection;

    public RayCast2D? WallDetection => _wallDetection;
    private Vector2 _wallDetectionOrigin;
    private Area2D? _attackArea;

    /// <summary>
    /// <para>Nodes in the attack range</para>
    /// <para>在攻击范围内的节点</para>
    /// </summary>
    private List<Node>? _nodesInTheAttackRange;

    /// <summary>
    /// <para>All nodes in the attack range</para>
    /// <para>在攻击范围内的全部节点</para>
    /// </summary>
    public Node[] NodesInTheAttackRange => _nodesInTheAttackRange?.ToArray() ?? Array.Empty<Node>();


    /// <summary>
    /// <para>Obstacle detection ray during attack</para>
    /// <para>攻击时的障碍物检测射线</para>
    /// </summary>
    /// <remarks>
    ///<para></para>
    ///<para>检测与目标点直接是否间隔墙壁</para>
    /// </remarks>
    private RayCast2D? _attackObstacleDetection;


    public RayCast2D? AttackObstacleDetection => _attackObstacleDetection;

    public override void _Ready()
    {
        base._Ready();
        _nodesInTheAttackRange = new List<Node>();
        _behaviorNode = GetNode<BehaviorNode>("Behavior");
        _wallDetection = GetNode<RayCast2D>("WallDetection");
        _attackArea = GetNode<Area2D>("AttackArea2D");
        if (ItemMarker2D != null)
        {
            _attackObstacleDetection = ItemMarker2D.GetNode<RayCast2D>("AttackObstacleDetection");
        }

        if (_attackArea != null)
        {
            //If true, the zone will detect objects or areas entering and leaving the zone.
            //如果为true，该区域将检测进出该区域的物体或区域。
            _attackArea.Monitoring = true;
            //Other areas can't detect our attack zone
            //其他区域不能检测到我们的攻击区域
            _attackArea.Monitorable = false;
            _attackArea.BodyEntered += EnterTheAttackArea;
            _attackArea.BodyExited += ExitTheAttackArea;
        }

        _wallDetectionOrigin = _wallDetection.TargetPosition;
        // var patrolBehaviorTree = new PatrolBehaviorTree();
        // patrolBehaviorTree.Character = this;
        // patrolBehaviorTree.Init();
        // _behaviorNode.Root = patrolBehaviorTree.Root;
    }

    private void EnterTheAttackArea(Node node)
    {
        _nodesInTheAttackRange?.Add(node);
    }

    private void ExitTheAttackArea(Node node)
    {
        _nodesInTheAttackRange?.Remove(node);
    }

    /// <summary>
    /// <para>Move left</para>
    /// <para>向左移动</para>
    /// </summary>
    public void MoveLeft()
    {
        var oldVelocity = Velocity;
        oldVelocity.X = -_movementSpeed;
        Velocity = oldVelocity;
    }

    /// <summary>
    /// <para>Move right</para>
    /// <para>向右移动</para>
    /// </summary>
    public void MoveRight()
    {
        var oldVelocity = Velocity;
        oldVelocity.X = _movementSpeed;
        Velocity = oldVelocity;
    }

    /// <summary>
    /// <para>Rotor</para>
    /// <para>转头</para>
    /// </summary>
    public void Rotor()
    {
        FacingLeft = !FacingLeft;
        Flip();
        //Change the direction of the wall detection
        //改变墙壁检测的方向
        if (_wallDetection == null) return;
        var newDirection = _wallDetectionOrigin;
        newDirection.X = FacingLeft ? -_wallDetectionOrigin.X : _wallDetectionOrigin.X;
        _wallDetection.TargetPosition = newDirection;
    }

    /// <summary>
    /// <para>stop moving</para>
    /// <para>停止移动</para>
    /// </summary>
    public void StopMoving()
    {
        Velocity = Vector2.Zero;
    }
}