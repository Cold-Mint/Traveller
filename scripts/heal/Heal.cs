using Godot;

namespace ColdMint.scripts.heal;

/// <summary>
/// <para>Health</para>
/// <para>健康值</para>
/// </summary>
/// <remarks>
///<para>Damage of meaning, please call <see cref="ColdMint.scripts.character.CharacterTemplate.Heal"/> method to restore character life value.</para>
///<para>伤害的反意，请调用<see cref="ColdMint.scripts.character.CharacterTemplate.Heal"/>方法使角色恢复生命值。</para>
/// </remarks>
public class Heal
{

    /// <summary>
    /// <para>HealAmount</para>
    /// <para>治疗量</para>
    /// </summary>
    public int HealAmount { get; set; }
    
    /// <summary>
    /// <para>Whether the damage text moves to the left</para>
    /// <para>伤害文本是否向左移动</para>
    /// </summary>
    public bool MoveLeft { get; set; }

    /// <summary>
    /// <para>Where does this treatment come from?</para>
    /// <para>这个治疗是来源于那里？</para>
    /// </summary>
    public Node2D? Source { get; set; }

}