namespace ColdMint.scripts.stateMachine;

/// <summary>
/// <para>IStateProcessor</para>
/// <para>状态处理器</para>
/// </summary>
public interface IStateProcessor
{
    /// <summary>
    /// <para>Enter the current state</para>
    /// <para>进入当前状态时</para>
    /// </summary>
    /// <param name="context"></param>
    void Enter(StateContext context);
    /// <summary>
    /// <para>Execution processor</para>
    /// <para>执行处理器</para>
    /// </summary>
    /// <param name="context"></param>
    void Execute(StateContext context);
    
    /// <summary>
    /// <para>When exiting a state</para>
    /// <para>退出某个状态时</para>
    /// </summary>
    /// <param name="context"></param>
    void Exit(StateContext context);
    
    /// <summary>
    /// <para>Gets the state to be processed by this processor</para>
    /// <para>获取此处理器要处理的状态</para>
    /// </summary>
    State State { get; }
}