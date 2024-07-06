using System.Collections.Generic;
using ColdMint.scripts.debug;

namespace ColdMint.scripts.stateMachine;

/// <summary>
/// <para>State machine template</para>
/// <para>状态机模板</para>
/// </summary>
public abstract class StateMachineTemplate : IStateMachine
{
    private StateContext? _context;
    private IStateProcessor? _activeStatusrocessor;

    public StateContext? Context
    {
        get => _context;
        set
        {
            if (value == null)
            {
                return;
            }

            if (_context != null)
            {
                _context.OnStateChange -= OnStateChange;
            }

            _context = value;
            value.OnStateChange += OnStateChange;
        }
    }

    private bool _isRunning;
    private Dictionary<State, IStateProcessor>? _processors;
    public bool IsRunning => _isRunning;

    /// <summary>
    /// <para>When the state in the context changes</para>
    /// <para>当上下文内的状态改变时</para>
    /// </summary>
    /// <param name="oldState"></param>
    /// <param name="newState"></param>
    private void OnStateChange(State oldState, State newState)
    {
        if (_context == null)
        {
            LogCat.LogError("state_machine_does_not_specify_context", label: LogCat.LogLabel.StateMachineTemplate);
            return;
        }

        if (_processors == null)
        {
            LogCat.LogError("state_machine_does_not_specify_processor", label: LogCat.LogLabel.StateMachineTemplate);
            return;
        }

        if (_processors.TryGetValue(oldState, out var processor))
        {
            processor.Exit(_context);
        }

        if (_processors.TryGetValue(newState, out processor))
        {
            processor.Enter(_context);
            _activeStatusrocessor = processor;
        }
        else
        {
            LogCat.LogErrorWithFormat("state_processor_not_found", label: LogCat.LogLabel.StateMachineTemplate,
                newState);
        }
    }

    /// <summary>
    /// <para>Registration status handler</para>
    /// <para>注册状态处理器</para>
    /// </summary>
    /// <param name="processor"></param>
    protected void RegisterProcessor(IStateProcessor processor)
    {
        _processors ??= new Dictionary<State, IStateProcessor>();
        if (!_processors.TryAdd(processor.State, processor))
        {
            LogCat.LogError("state_processor_already_registered", label: LogCat.LogLabel.StateMachineTemplate);
        }
    }

    public void Start()
    {
        if (_isRunning)
        {
            LogCat.LogError("try_to_open_state_machine_that_is_on", label: LogCat.LogLabel.StateMachineTemplate);
            return;
        }

        if (Context == null)
        {
            LogCat.LogError("state_machine_does_not_specify_context", label: LogCat.LogLabel.StateMachineTemplate);
            return;
        }

        OnStart(Context);
        _activeStatusrocessor = _processors?[Context.CurrentState];
        _isRunning = true;
    }

    /// <summary>
    /// <para>When the state machine is turned on</para>
    /// <para>在状态机开启时</para>
    /// </summary>
    /// <remarks>
    ///<para>Register the status handler in this method<see cref="RegisterProcessor"/>.</para>
    ///<para>请在此方法内注册状态处理器<see cref="RegisterProcessor"/>。</para>
    /// </remarks>
    protected abstract void OnStart(StateContext context);

    /// <summary>
    /// <para>When the state switch is off</para>
    /// <para>在状态机关闭时</para>
    /// </summary>
    protected virtual void OnStop()
    {
    }

    public void Stop()
    {
        if (!_isRunning)
        {
            return;
        }

        _isRunning = false;
        OnStop();
    }

    public void Execute()
    {
        if (!_isRunning)
        {
            return;
        }

        if (Context == null)
        {
            LogCat.LogError("state_machine_does_not_specify_context", label: LogCat.LogLabel.StateMachineTemplate);
            return;
        }

        if (_activeStatusrocessor == null)
        {
            LogCat.LogError("state_machine_does_not_specify_active_processor",
                label: LogCat.LogLabel.StateMachineTemplate);
            return;
        }

        _activeStatusrocessor.Execute(Context);
    }
}