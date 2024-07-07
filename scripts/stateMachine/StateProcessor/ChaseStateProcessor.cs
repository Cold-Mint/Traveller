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

        //Get the first enemy to enter the reconnaissance range.
        //获取第一次进入侦察范围的敌人。
        var enemy = aiCharacter.GetFirstEnemyInScoutArea();
        if (enemy == null)
        {
            //No more enemies. Return to previous status.
            //没有敌人了，返回上一个状态。
            aiCharacter.HiddenQuery();
            aiCharacter.SetTargetPosition(aiCharacter.GlobalPosition);
            LogCat.Log("chase_no_enemy", label: LogCat.LogLabel.ChaseStateProcessor);
            context.CurrentState = context.PreviousState;
        }
        else
        {
            //Set the position of the enemy entering the range to the position we are going to.
            //将进入范围的敌人位置设置为我们要前往的位置。
            aiCharacter.SetTargetPosition(enemy.GlobalPosition);
            aiCharacter.DispladyQuery();
        }
    }

    public override State State => State.Chase;
}