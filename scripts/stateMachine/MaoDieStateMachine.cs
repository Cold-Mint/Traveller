using ColdMint.scripts.stateMachine.StateProcessor;

namespace ColdMint.scripts.stateMachine;

public class MaoDieStateMachine : StateMachineTemplate
{
    protected override State OnStart(StateContext context)
    {
        var chaseStateProcessor = new MaoDieChaseStateProcessor();
        RegisterProcessor(chaseStateProcessor);
        return chaseStateProcessor.State;
    }
}