using ColdMint.scripts.character;
using ColdMint.scripts.loader.uiLoader;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts;

/// <summary>
/// <para>Dependencies on the runtime of the game scene</para>
/// <para>游戏场景运行时的依赖</para>
/// </summary>
public static class GameSceneDepend
{
    public static GameGuiTemplate? GameGuiTemplate { get; set; }

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
            if (GameGuiTemplate?.MiniMap != null)
            {
                GameGuiTemplate.MiniMap.OwnerNode = _player;
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
    /// <para>SpellContainer</para>
    /// <para>法术容器</para>
    /// </summary>
    public static Node2D? SpellContainer { get; set; }

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
    /// <para>PickAbleContainer</para>
    /// <para>可拾捡物容器</para>
    /// </summary>
    public static Node2D? PickAbleContainer { get; set; }

    /// <summary>
    /// <para>AICharacterContainer</para>
    /// <para>AICharacter角色</para>
    /// </summary>
    public static Node2D? AiCharacterContainer { get; set; }


    /// <summary>
    /// <para>DynamicUiGroup</para>
    /// <para>动态生成的Ui组</para>
    /// </summary>
    /// <remarks>
    ///<para>Dynamically generated Ui objects will be placed under this node</para>
    ///<para>动态生成的Ui对象将放置在此节点下</para>
    /// </remarks>
    public static UiGroup? DynamicUiGroup { get; set; }


    /// <summary>
    /// <para>Whether the player's mouse is hovering over GUI furniture</para>
    /// <para>玩家的鼠标是否悬浮在GUI家具上</para>
    /// </summary>
    public static bool IsMouseOverFurnitureGui;


    /// <summary>
    /// <para>Whether the mouse is suspended over the item slot</para>
    /// <para>鼠标是否悬浮在物品槽上</para>
    /// </summary>
    public static bool IsMouseOverItemSlotNode;


    /// <summary>
    /// <para>ShowMiniMap</para>
    /// <para>显示迷你地图</para>
    /// </summary>
    public static void ShowMiniMap()
    {
        if (GameGuiTemplate?.MiniMap == null)
        {
            return;
        }
        if (GameGuiTemplate.MiniMap.Visible)
        {
            return;
        }
        GameGuiTemplate.MiniMapAnimationPlayer?.Play(name: "show");
    }


    /// <summary>
    /// <para>HideMiniMap</para>
    /// <para>隐藏迷你地图</para>
    /// </summary>
    public static void HideMiniMap()
    {
        if (GameGuiTemplate?.MiniMap == null)
        {
            return;
        }
        if (GameGuiTemplate.MiniMap.Visible)
        {
            GameGuiTemplate.MiniMapAnimationPlayer?.Play(name: "hide");
        }
    }
}