using ColdMint.scripts.character;
using ColdMint.scripts.inventory;
using ColdMint.scripts.map.events;
using Godot;

namespace ColdMint.scripts;

/// <summary>
/// <para>The node holder within the game scene</para>
/// <para>游戏场景内的节点持有者</para>
/// </summary>
public static class GameSceneNodeHolder
{
    private static Player? _player;

    /// <summary>
    /// <para>Player instances within the game scene</para>
    /// <para>游戏场景内的玩家实例</para>
    /// </summary>
    public static Player? Player
    {
        get => _player;
        set
        {
            _player = value;
            //Broadcast the event to the outside when the player instance changes.
            //当玩家实例改变时，向外广播事件。
            var playerInstanceChangeEvent = new PlayerInstanceChangeEvent
            {
                PlayerInstance = _player
            };
            EventManager.PlayerInstanceChangeEvent?.Invoke(playerInstanceChangeEvent);
        }
    }

    /// <summary>
    /// <para>WeaponContainer</para>
    /// <para>武器容器</para>
    /// </summary>
    public static Node2D? WeaponContainer { get; set; }

    /// <summary>
    /// <para>PacksackContainer</para>
    /// <para>背包容器</para>
    /// </summary>
    public static Node2D? PacksackContainer { get; set; }

    /// <summary>
    /// <para>PlayerContainer</para>
    /// <para>玩家容器</para>
    /// </summary>
    public static Node2D? PlayerContainer { get; set; }

    /// <summary>
    /// <para>AICharacterContainer</para>
    /// <para>AICharacter角色</para>
    /// </summary>
    public static Node2D? AiCharacterContainer { get; set; }

    /// <summary>
    /// <para>HotBar</para>
    /// <para>快捷栏</para>
    /// </summary>
    public static HotBar? HotBar { get; set; }


    /// <summary>
    /// <para>Health Bar UI</para>
    /// <para>健康条UI</para>
    /// </summary>
    public static HealthBarUi? HealthBarUi { get; set; }

    /// <summary>
    /// <para>operation tip</para>
    /// <para>操作提示</para>
    /// </summary>
    public static RichTextLabel? OperationTipLabel { get; set; }

    /// <summary>
    /// <para>BackpackUiContainer</para>
    /// <para>背包Ui容器</para>
    /// </summary>
    /// <remarks>
    ///<para>The knapsack Ui container houses the container of the knapsack ui node. When a user uses a backpack, the node to which his backpack is attached is displayed from within the backpack ui container.</para>
    ///<para>背包Ui容器内存放的是背包ui节点的容器。当用户使用背包时，会从背包ui容器内将其背包对于的节点展示出来。</para>
    /// </remarks>
    public static Control? BackpackUiContainer { get; set; }
}