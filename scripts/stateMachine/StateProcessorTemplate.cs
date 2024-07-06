using Godot;

namespace ColdMint.scripts.stateMachine;

/// <summary>
/// <para>StateProcessorTemplate</para>
/// <para>状态处理器模板</para>
/// </summary>
public abstract class StateProcessorTemplate : IStateProcessor
{
    public virtual void Enter(StateContext context)
    {
    }

    public void Execute(StateContext context)
    {
        if (context.Owner == null)
        {
            return;
        }

        OnExecute(context, context.Owner);
    }

    /// <summary>
    /// <para>When executed</para>
    /// <para>当执行时</para>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="owner"></param>
    protected abstract void OnExecute(StateContext context, Node owner);

    public virtual void Exit(StateContext context)
    {
    }

    public abstract State State { get; }
}