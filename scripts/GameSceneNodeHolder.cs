using ColdMint.scripts.character;
using ColdMint.scripts.inventory;
using Godot;

namespace ColdMint.scripts;

/// <summary>
/// <para>The node holder within the game scene</para>
/// <para>游戏场景内的节点持有者</para>
/// </summary>
public static class GameSceneNodeHolder
{
    /// <summary>
    /// <para>Player instances within the game scene</para>
    /// <para>游戏场景内的玩家实例</para>
    /// </summary>
    public static Player? Player { get; set; }

    /// <summary>
    /// <para>WeaponContainer</para>
    /// <para>武器容器</para>
    /// </summary>
    public static Node2D WeaponContainer { get; set; }
    
    public static HotBar HotBar { get; set; }


    public static HealthBarUi HealthBarUi { get; set; }
    public static Label OperationTipLabel { get; set; }
}