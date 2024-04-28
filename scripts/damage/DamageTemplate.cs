using System;
using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.damage;

/// <summary>
/// <para>DamageTemplate</para>
/// <para>伤害模板</para>
/// </summary>
public abstract class DamageTemplate
{
    /// <summary>
    /// <para>Damage must be assigned a certain value</para>
    /// <para>伤害必须指定确定的数值</para>
    /// </summary>
    public int Damage => _damage;

    /// <summary>
    /// <para>Critical Hit probability (unit: percent)</para>
    /// <para>暴击几率(单位：百分比)</para>
    /// </summary>
    public int CriticalStrikeProbability = 5;

    private int _damage;

    private bool _isCriticalStrike;
    
    /// <summary>
    /// <para></para>
    /// <para>攻击者</para>
    /// </summary>
    public Node2D Attacker { get; set; }

    /// <summary>
    /// <para>Whether the damage text moves to the left</para>
    /// <para>伤害文本是否向左移动</para>
    /// </summary>
    public bool MoveLeft { get; set; }

    /// <summary>
    /// <para>Create actual damage with maximum and minimum values</para>
    /// <para>通过最大值和最小值创建实际伤害</para>
    /// </summary>
    public void CreateDamage()
    {
        _damage = GD.RandRange(MinDamage, MaxDamage);
        _isCriticalStrike = GD.RandRange(1, 100) <= CriticalStrikeProbability;
        if (_isCriticalStrike)
        {
            _damage = (int)Math.Round(_damage * Config.CriticalHitMultiplier);
        }
    }

    /// <summary>
    /// <para>Maximum injury</para>
    /// <para>最大伤害</para>
    /// </summary>
    public int MaxDamage { get; set; }

    /// <summary>
    /// <para>Minimum damage value</para>
    /// <para>最小伤害值</para>
    /// </summary>
    public int MinDamage { get; set; }


    /// <summary>
    /// <para>Whether the damage is critical</para>
    /// <para>本次伤害是否为暴击</para>
    /// </summary>
    public bool IsCriticalStrike => _isCriticalStrike;

    /// <summary>
    /// <para>Types of damage</para>
    /// <para>伤害的类型</para>
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// <para>An event performed before harming the enemy</para>
    /// <para>在伤害敌人之前执行的事件</para>
    /// </summary>
    public Action<Node2D> BeforeDamage;

    /// <summary>
    /// <para>After damaging the enemy</para>
    /// <para>在伤害敌人之后</para>
    /// </summary>
    public Action<Node2D> AfterDamage;
}