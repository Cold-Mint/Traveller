using ColdMint.scripts.inventory;
using ColdMint.scripts.map.miniMap;
using Godot;

namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>Graphical user page of the game scene</para>
/// <para>游戏场景的图像用户页面</para>
/// </summary>
public interface IGameGui
{
    /// <summary>
    /// <para>FPSLabel</para>
    /// <para>获取FPS标签</para>
    /// </summary>
    Label? FpsLabel { get; }

    /// <summary>
    /// <para>MiniMap</para>
    /// <para>获取迷你地图</para>
    /// </summary>
    MiniMap? MiniMap { get; }

    /// <summary>
    /// <para>HealthBar</para>
    /// <para>健康条</para>
    /// </summary>
    HealthBarUi? HealthBar { get; }

    /// <summary>
    /// <para>HotBar</para>
    /// <para>快捷栏</para>
    /// </summary>
    HotBar? HotBar { get; }

    /// <summary>
    /// <para>MiniMapAnimationPlayer</para>
    /// <para>迷你地图的动画播放器</para>
    /// </summary>
    AnimationPlayer? MiniMapAnimationPlayer { get; }

    /// <summary>
    /// <para>LeftButton</para>
    /// <para>左侧移动按钮</para>
    /// </summary>
    TouchScreenButton? LeftButton { get; }

    /// <summary>
    /// <para>RightButton</para>
    /// <para>右侧移动按钮</para>
    /// </summary>
    TouchScreenButton? RightButton { get; }


    /// <summary>
    /// <para>Jump button</para>
    /// <para>跳跃按钮</para>
    /// </summary>
    TouchScreenButton? JumpButton { get; }

    /// <summary>
    /// <para>Pick Button</para>
    /// <para>捡起按钮</para>
    /// </summary>
    TouchScreenButton? PickButton { get; }
    
    /// <summary>
    /// <para>Throw Button</para>
    /// <para>抛出按钮</para>
    /// </summary>
    RockerButton? ThrowButton { get; }

}