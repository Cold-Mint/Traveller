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
                new Godot.Vector2(Config.CellSize * 3, 0),
                new Godot.Vector2(-Config.CellSize * 3, 0),
            ]
        };
        RegisterProcessor(patrolStateProcessor);
        var chaseStateProcessor = new ChaseStateProcessor();
        RegisterProcessor(chaseStateProcessor);
        var lookForWeaponProcessor = new LookForWeaponProcessor();
        RegisterProcessor(lookForWeaponProcessor);
        var fleeProcessor = new FleeProcessor();
        RegisterProcessor(fleeProcessor);
        var attackStateProcessor = new AttackStateProcessor();
        RegisterProcessor(attackStateProcessor);
    }
}