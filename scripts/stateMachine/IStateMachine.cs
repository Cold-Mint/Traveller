namespace ColdMint.scripts.stateMachine;

/// <summary>
/// <para>IStateMachine</para>
/// <para>状态机</para>
/// </summary>
public interface IStateMachine
{
    /// <summary>
    /// <para>StateContext</para>
    /// <para>状态机上下文</para>
    /// </summary>
    /// <remarks>
    ///<para>The state machine holds the context</para>
    ///<para>状态机持有上下文</para>
    /// </remarks>
    StateContext? Context { get; set; }
    
    /// <summary>
    /// <para>In operation or not</para>
    /// <para>是否运行中</para>
    /// </summary>
    bool IsRunning { get;}

    /// <summary>
    /// <para>Open state machine</para>
    /// <para>开启状态机</para>
    /// </summary>
    void Start();
    
    
    /// <summary>
    /// <para>Stop state machine</para>
    /// <para>停止状态机</para>
    /// </summary>
    void Stop();

    /// <summary>
    /// <para>execute</para>
    /// <para>执行</para>
    /// </summary>
    void Execute();

}