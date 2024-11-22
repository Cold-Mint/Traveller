using Godot;

namespace ColdMint.scripts.damage;

/// <summary>
/// <para>Fixed Damage</para>
/// <para>固定伤害</para>
/// </summary>
public class FixedDamage(int damage) : IDamage
{
    public int Damage
    {
        get => damage;
    }
    public bool IsCriticalStrike { get; set; }
    public int Type { get; set; }
    public bool MoveLeft { get; set; }
    public Node2D? Attacker { get; set; }
}