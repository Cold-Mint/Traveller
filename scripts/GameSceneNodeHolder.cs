using ColdMint.scripts.character;
using ColdMint.scripts.inventory;
using ColdMint.scripts.loader.uiLoader;
using ColdMint.scripts.map.events;
using ColdMint.scripts.utils;
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
    /// <para>When the mouse enters the scope of a character, it is considered a target</para>
    /// <para>鼠标进入到某个角色的范围内时，会将其视作目标</para>
    /// </summary>
    public static Node2D? TemporaryTargetNode { get; set; }

    /// <summary>
    /// <para>ProjectileContainer</para>
    /// <para>抛射体容器</para>
    /// </summary>
    public static Node2D? ProjectileContainer { get; set; }

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


    /// <summary>
    /// <para>Hide the knapsack node in the knapsack Ui if the knapsack UI is displayed</para>
    /// <para>如果背包Ui处于显示状态，那么隐藏背包UI内的背包节点</para>
    /// </summary>
    public static void HideBackpackUiContainerIfVisible()
    {
        if (BackpackUiContainer == null)
        {
            return;
        }

        if (!BackpackUiContainer.Visible)
        {
            return;
        }

        NodeUtils.ForEachNode<PacksackUi>(BackpackUiContainer, node =>
        {
            //If the child node is not visible, the traversal continues.
            //如果子节点不可见，则继续遍历。
            if (!node.Visible)
                return false;
            //Until you find a visible node, hide it, and return true, ending the loop.
            //直到找到可见的节点，隐藏该节点，然后返回true，结束遍历。
            node.Hide();
            return true;
        });
    }
}