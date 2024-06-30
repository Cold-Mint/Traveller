using System;
using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.stateMachine;

/// <summary>
/// <para>Context of the state machine</para>
/// <para>状态机的上下文环境</para>
/// </summary>
public class StateContext
{
    private State _currentState;

    /// <summary>
    /// <para>The state context holds the current state</para>
    /// <para>状态上下文持有当前状态</para>
    /// </summary>
    public State CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState == value)
            {
                LogCat.LogWarning("try_to_set_the_same_state");
                return;
            }

            OnStateChange?.Invoke(_currentState, value);
            _currentState = value;
        }
    }

    /// <summary>
    /// <para>When the state changes</para>
    /// <para>当状态改变时</para>
    /// </summary>
    public Action<State, State>? OnStateChange;

    /// <summary>
    /// <para>owner</para>
    /// <para>主人</para>
    /// </summary>
    public Node? Owner { get; set; }
}