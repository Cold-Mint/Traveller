using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.stateMachine.StateProcessor;

/// <summary>
/// <para>Chasing state processor</para>
/// <para>追击状态处理器</para>
/// </summary>
public class ChaseStateProcessor : StateProcessorTemplate
{
    protected override void OnExecute(StateContext context, Node owner)
    {
        if (owner is not AiCharacter aiCharacter)
        {
            return;
        }

        //Get the first enemy to enter the attack range.
        //获取第一次进入攻击范围的敌人。
        var enemy = aiCharacter.GetFirstEnemy();
        if (enemy == null)
        {
            //No more enemies. Return to previous status.
            //没有敌人了，返回上一个状态。
            LogCat.Log("chase_no_enemy", label: LogCat.LogLabel.ChaseStateProcessor);
            context.CurrentState = context.PreviousState;
        }
        else
        {
            //Chase the enemy.
            //追击敌人。
            aiCharacter.SetTargetPosition(enemy.GlobalPosition);
        }
    }

    public override State State => State.Chase;
}