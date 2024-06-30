using ColdMint.scripts.stateMachine.StateProcessor;

namespace ColdMint.scripts.stateMachine;

/// <summary>
/// <para>State machine for patrollers</para>
/// <para>适用于巡逻者的状态机</para>
/// </summary>
public class PatrolStateMachine : StateMachineTemplate
{
    protected override void OnStart(StateContext context)
    {
        RegisterProcessor(new PatrolStateProcessor());
    }
}