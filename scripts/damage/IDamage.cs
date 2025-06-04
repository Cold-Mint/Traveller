using Godot;

namespace ColdMint.scripts.damage;

public interface IDamage
{
    /// <summary>
    /// <para>Damage must be assigned a certain value</para>
    /// <para>伤害必须指定确定的数值</para>
    /// </summary>
    public int Damage { get; }

    /// <summary>
    /// <para>Whether the damage is critical</para>
    /// <para>本次伤害是否为暴击</para>
    /// </summary>
    public bool IsCriticalStrike { get; set; }

    /// <summary>
    /// <para>Types of damage</para>
    /// <para>伤害的类型</para>
    /// </summary>
    public Config.DamageType Type { get; set; }

    /// <summary>
    /// <para>Whether the damage text moves to the left</para>
    /// <para>伤害文本是否向左移动</para>
    /// </summary>
    public bool MoveLeft { get; set; }

    /// <summary>
    /// <para></para>
    /// <para>攻击者</para>
    /// </summary>
    public Node2D? Attacker { get; set; }

}