using System.Collections.Generic;
using ColdMint.scripts.camp;
using ColdMint.scripts.stateMachine;
using Godot;

namespace ColdMint.scripts.character;

/// <summary>
/// <para>The role played by computers</para>
/// <para>由电脑扮演的角色</para>
/// </summary>
public sealed partial class AiCharacter : CharacterTemplate
{
    //Used to detect rays on walls
    //用于检测墙壁的射线
    private RayCast2D? _wallDetection;

    public RayCast2D? WallDetection => _wallDetection;
    private Vector2 _wallDetectionOrigin;
    private Area2D? _attackArea;

    /// <summary>
    /// <para>All enemies within striking distance</para>
    /// <para>在攻击范围内的所有敌人</para>
    /// </summary>
    private List<CharacterTemplate>? _enemyInTheAttackRange;


    /// <summary>
    /// <para>Obstacle detection ray during attack</para>
    /// <para>攻击时的障碍物检测射线</para>
    /// </summary>
    /// <remarks>
    ///<para></para>
    ///<para>检测与目标点直接是否间隔墙壁</para>
    /// </remarks>
    private RayCast2D? _attackObstacleDetection;


    /// <summary>
    /// <para>Navigation agent</para>
    /// <para>导航代理</para>
    /// </summary>
    public NavigationAgent2D? NavigationAgent2D { get; set; }


    public IStateMachine? StateMachine { get; set; }


    public RayCast2D? AttackObstacleDetection => _attackObstacleDetection;

    public override void _Ready()
    {
        base._Ready();
        _enemyInTheAttackRange = new List<CharacterTemplate>();
        _wallDetection = GetNode<RayCast2D>("WallDetection");
        _attackArea = GetNode<Area2D>("AttackArea2D");
        NavigationAgent2D = GetNode<NavigationAgent2D>("NavigationAgent2D");
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
        StateMachine = new PatrolStateMachine();
        StateMachine.Context = new StateContext
        {
            CurrentState = State.Patrol,
            Owner = this
        };
        if (StateMachine != null)
        {
            StateMachine.Start();
        }
    }

    /// <summary>
    /// <para>EnemyDetected</para>
    /// <para>是否发现敌人</para>
    /// </summary>
    /// <returns>
    ///<para>Have you spotted the enemy?</para>
    ///<para>是否发现敌人</para>
    /// </returns>
    public bool EnemyDetected()
    {
        if (_enemyInTheAttackRange == null)
        {
            return false;
        }

        return _enemyInTheAttackRange.Count > 0;
    }

    /// <summary>
    /// <para>Get the first enemy to enter range</para>
    /// <para>获取第一个进入范围的敌人</para>
    /// </summary>
    /// <returns></returns>
    public CharacterTemplate? GetFirstEnemy()
    {
        if (_enemyInTheAttackRange == null || _enemyInTheAttackRange.Count == 0)
        {
            return null;
        }
        return _enemyInTheAttackRange[0];
    }

    protected override void HookPhysicsProcess(ref Vector2 velocity, double delta)
    {
        StateMachine?.Execute();
        if (NavigationAgent2D != null && IsOnFloor())
        {
            var nextPathPosition = NavigationAgent2D.GetNextPathPosition();
            var direction = (nextPathPosition - GlobalPosition).Normalized();
            velocity = direction * Config.CellSize * Speed;
        }
    }

    private void EnterTheAttackArea(Node node)
    {
        if (node == this)
        {
            //The target can't be yourself.
            //攻击目标不能是自己。
            return;
        }

        if (node is CharacterTemplate characterTemplate)
        {
            //Determine if damage can be done between factions
            //判断阵营间是否可造成伤害
            var camp = CampManager.GetCamp(CampId);
            var enemyCamp = CampManager.GetCamp(characterTemplate.CampId);
            if (enemyCamp != null && camp != null)
            {
                var canCause = CampManager.CanCauseHarm(camp, enemyCamp);
                if (canCause)
                {
                    _enemyInTheAttackRange?.Add(characterTemplate);
                }
            }
        }
    }

    private void ExitTheAttackArea(Node node)
    {
        if (node == this)
        {
            return;
        }

        if (node is CharacterTemplate characterTemplate)
        {
            _enemyInTheAttackRange?.Remove(characterTemplate);
        }
    }


    /// <summary>
    /// <para>Set target location</para>
    /// <para>设置目标位置</para>
    /// </summary>
    /// <param name="targetPosition"></param>
    public void SetTargetPosition(Vector2 targetPosition)
    {
        if (NavigationAgent2D == null)
        {
            return;
        }

        NavigationAgent2D.TargetPosition = targetPosition;
    }


    public override void _ExitTree()
    {
        base._ExitTree();
        if (_attackArea != null)
        {
            _attackArea.BodyEntered -= EnterTheAttackArea;
            _attackArea.BodyExited -= ExitTheAttackArea;
        }

        if (StateMachine != null)
        {
            StateMachine.Stop();
        }
    }
}