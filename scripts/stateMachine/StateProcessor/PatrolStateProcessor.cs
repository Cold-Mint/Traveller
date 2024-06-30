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
    protected override void OnExecute(StateContext context, Node owner)
    {
        if (owner is not AiCharacter aiCharacter)
        {
            return;
        }
        
        aiCharacter.MoveLeft();
    }

    public override State State => State.Patrol;
}