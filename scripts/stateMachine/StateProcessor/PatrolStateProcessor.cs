using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.stateMachine.StateProcessor;

/// <summary>
/// <para>PatrolStateProcessor</para>
/// <para>巡逻状态处理器</para>
/// </summary>
public class PatrolStateProcessor : StateProcessorTemplate
{
    public Vector2[]? Points { get; set; }
    /// <summary>
    /// <para>Whether to guard the origin</para>
    /// <para>是否需要守护原点</para>
    /// </summary>
    /// <remarks>
    ///<para>When empty by default, PatrolStateProcessor will take the first point where the character touches the ground as the origin. This property handles whether or not the origin is "guarded" when the character is attracted to another character, such as chasing an enemy, and switches back to patrol mode. If set to true, the role tries to return to the origin, otherwise, a new origin is assigned.</para>
    ///<para>默认清空下，PatrolStateProcessor会将角色与地面接触的第一个位置当作原点。这个属性用来处理当角色被其他角色所吸引，（例如追击敌人）转换回巡逻模式，是否“守护”原点。如果设置为true，则角色会尝试返回原点，否则，将分配新的原点。</para>
    /// </remarks>
    public bool Guard { get; set; }

    private int _index;
    private Vector2? _originPosition;

    public override void Enter(StateContext context)
    {
        if (!Guard)
        {
            //Reset the origin when transitioning to patrol.
            //转换到巡逻状态时重置原点。
            _originPosition = null;
        }
    }

    protected override void OnExecute(StateContext context, Node owner)
    {
        if (owner is not AiCharacter aiCharacter)
        {
            return;
        }

        if (aiCharacter.ScoutEnemyDetected())
        {
            //Once the enemy enters the reconnaissance range, we first see if the character has a weapon, if so, then pursue the enemy, otherwise, the patrol state changes to looking for weapons.
            //发现敌人进入侦察范围后，我们先看角色是否持有武器，如果有，那么追击敌人，否则，巡逻状态转换为寻找武器。
            // if (aiCharacter.CurrentItem is WeaponTemplate weaponTemplate)
            // {
            context.CurrentState = State.Chase;
            // }
            // else
            // {
            //     context.CurrentState = State.LookForWeapon;
            // }

            LogCat.Log("patrol_enemy_detected", label: LogCat.LogLabel.PatrolStateProcessor);
            return;
        }

        if (Points == null || Points.Length == 0)
        {
            //There are no patrol points.
            //没有巡逻点。
            LogCat.LogError("no_points", label: LogCat.LogLabel.PatrolStateProcessor);
            return;
        }

        if (_originPosition == null)
        {
            //If there is no origin, then wait for the character to collide with the ground to set it as the origin.
            //如果没有原点，那么等待角色与地面碰撞时将其设置为原点。
            if (!aiCharacter.IsOnFloor())
            {
                LogCat.LogWarning("patrol_not_on_floor", LogCat.LogLabel.PatrolStateProcessor);
                return;
            }

            _originPosition = aiCharacter.GlobalPosition;
            LogCat.LogWithFormat("patrol_origin_position", LogCat.LogLabel.PatrolStateProcessor,
                _originPosition);
        }

        var point = _originPosition + Points[_index];
        var distance = aiCharacter.GlobalPosition.DistanceTo(point.Value);
        if (distance < 5)
        {
            //No need to actually come to the patrol point, we just need a distance to get close.
            //无需真正的来到巡逻点，我们只需要一个距离接近了就可以了。
            LogCat.LogWithFormat("patrol_arrival_point", LogCat.LogLabel.PatrolStateProcessor, point);
            _index++;
            if (_index >= Points.Length)
            {
                _index = 0;
            }
        }
        else
        {
            LogCat.LogWithFormat("patrol_to_next_point", label: LogCat.LogLabel.PatrolStateProcessor, point,
                aiCharacter.GlobalPosition, Points[_index],
                distance);
            aiCharacter.SetTargetPosition(point.Value);
        }
    }


    public override State State => State.Patrol;
}