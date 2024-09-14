using ColdMint.scripts.character;
using ColdMint.scripts.inventory;
using ColdMint.scripts.map.miniMap;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts;

/// <summary>
/// <para>Dependencies on the runtime of the game scene</para>
/// <para>游戏场景运行时的依赖</para>
/// </summary>
public static class GameSceneDepend
{
    /// <summary>
    /// <para>MiniMap</para>
    /// <para>迷你地图</para>
    /// </summary>
    public static MiniMap? MiniMap { get; set; }

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
            if (MiniMap != null)
            {
                MiniMap.OwnerNode = _player;
            }
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
    public static UiGroup? BackpackUiContainer { get; set; }
}