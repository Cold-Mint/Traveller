using System;
using ColdMint.scripts.character;
using Godot;

namespace ColdMint.scripts.stateMachine.StateProcessor;

/// <summary>
/// <para>AttackStateProcessor</para>
/// <para>攻击状态处理器</para>
/// </summary>
public class AttackStateProcessor : StateProcessorTemplate
{

    /// <summary>
    /// <para>Consecutive attacks</para>
    /// <para>连续攻击次数</para>
    /// </summary>
    private int _consecutiveAttacks;

    /// <summary>
    /// <para>Max number of consecutive attacks</para>
    /// <para>最大连续攻击次数</para>
    /// </summary>
    /// <remarks>
    ///<para>When this value is reached, the attack needs to be paused for a period of time.</para>
    ///<para>到达此值后需要暂停一段时间攻击。</para>
    /// </remarks>
    public int MaxConsecutiveAttacks = 3;

    /// <summary>
    /// <para>How long to pause after the maximum number of attacks is reached</para>
    /// <para>到达最大攻击次数后停顿多长时间</para>
    /// </summary>
    public TimeSpan PauseTimeSpan = TimeSpan.FromSeconds(3);

    /// <summary>
    /// <para>Time of next attack</para>
    /// <para>下次攻击时间</para>
    /// </summary>
    private DateTime _nextAttackTime = DateTime.UtcNow;
    protected override void OnExecute(StateContext context, Node owner)
    {
        var now = DateTime.UtcNow;
        if (now < _nextAttackTime)
        {
            return;
        }
        if (owner is not AiCharacter aiCharacter)
        {
            return;
        }
        var enemy = aiCharacter.GetFirstEnemyInScoutArea();
        if (enemy == null)
        {
            context.CurrentState = context.PreviousState;
            return;
        }
        var canAttackEnemy = aiCharacter.GetFirstEnemyInAttackArea();
        if (canAttackEnemy == null)
        {
            context.CurrentState = context.PreviousState;
            return;
        }
        aiCharacter.DispladyPlaint();
        aiCharacter.HideQuery();
        if (aiCharacter.UseItem(enemy.GlobalPosition))
        {
            _consecutiveAttacks++;
            if (_consecutiveAttacks >= MaxConsecutiveAttacks)
            {
                _consecutiveAttacks = 0;
                _nextAttackTime = now + PauseTimeSpan;
            }
        }
        aiCharacter.AimTheCurrentItemAtAPoint(enemy.GlobalPosition);
    }

    public override State State => State.Attack;
}