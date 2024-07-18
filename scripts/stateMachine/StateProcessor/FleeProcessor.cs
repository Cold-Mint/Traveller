using System;
using ColdMint.scripts.character;
using Godot;

namespace ColdMint.scripts.stateMachine.StateProcessor;

/// <summary>
/// <para>Escape state processor</para>
/// <para>逃跑状态处理器</para>
/// </summary>
public class FleeProcessor : StateProcessorTemplate
{
    /// <summary>
    /// <para>When to return to enemy free status</para>
    /// <para>何时恢复到没有敌人的状态</para>
    /// </summary>
    private DateTime? _endTime;

    /// <summary>
    /// <para>When away from the enemy, how long to return to normal state</para>
    /// <para>当远离敌人后，多长时间恢复到正常状态</para>
    /// </summary>
    public TimeSpan RecoveryTimeSpan { get; set; } = TimeSpan.FromMilliseconds(300);

    protected override void OnExecute(StateContext context, Node owner)
    {
        if (owner is not AiCharacter aiCharacter)
        {
            return;
        }

        var enemy = aiCharacter.GetFirstEnemyInScoutArea();
        if (enemy == null)
        {
            //There are no enemies left.
            //没有敌人了
            if (_endTime == null)
            {
                _endTime = DateTime.Now + RecoveryTimeSpan;
                return;
            }

            if (DateTime.Now > _endTime)
            {
                //Recovery time, end status.
                //恢复时间，结束状态。
                context.CurrentState = State.Patrol;
            }
        }
        else
        {
            //Enemies
            //有敌人
            //To calculate the escape direction, the vector of the enemy pointing to the character is the escape direction.
            //计算逃跑方向，敌人指向角色的向量为逃跑方向。
            _endTime = null;
            var direction = aiCharacter.GlobalPosition - enemy.GlobalPosition;
            aiCharacter.SetTargetPosition(aiCharacter.GlobalPosition + direction);
        }
    }

    public override State State => State.Flee;
}