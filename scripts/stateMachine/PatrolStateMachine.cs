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
        var patrolStateProcessor = new PatrolStateProcessor
        {
            Points =
            [
                new Godot.Vector2(100, 0),
                new Godot.Vector2(-100, 0),
                new Godot.Vector2(50, 0),
                new Godot.Vector2(-50, 0),
                new Godot.Vector2(0, 0)
            ]
        };
        RegisterProcessor(patrolStateProcessor);
    }
}