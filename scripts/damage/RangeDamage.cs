using System;
using Godot;

namespace ColdMint.scripts.damage;

/// <summary>
/// <para>RangeDamage</para>
/// <para>区间伤害</para>
/// </summary>
public class RangeDamage : IDamage
{
    /// <summary>
    /// <para>Critical Hit probability (unit: percent)</para>
    /// <para>暴击几率(单位：百分比)</para>
    /// </summary>
    public int CriticalStrikeProbability = 5;

    private int _damage;

    private bool _isCriticalStrike;

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


    public int Damage
    {
        get => _damage;
    }
    public bool IsCriticalStrike { get; set; }
    public int Type { get; set; }
    public bool MoveLeft { get; set; }
    public Node2D? Attacker { get; set; }
}