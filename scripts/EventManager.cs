using System;
using ColdMint.scripts.map.events;

namespace ColdMint.scripts;

/// <summary>
/// <para>EventManager</para>
/// <para>事件管理器</para>
/// </summary>
public static class EventManager
{
    /// <summary>
    /// <para>Event when the AI character is generated</para>
    /// <para>AI角色生成事件</para>
    /// </summary>
    public static Action<AiCharacterGenerateEvent>? AiCharacterGenerateEvent;

    /// <summary>
    /// <para>Game Over Event</para>
    /// <para>游戏结束事件</para>
    /// </summary>
    public static Action<GameOverEvent>? GameOverEvent;

    /// <summary>
    /// <para>Events when the game is replayed</para>
    /// <para>游戏重玩时的事件</para>
    /// </summary>
    public static Action<GameReplayEvent>? GameReplayEvent;

    /// <summary>
    /// <para>Map starts generating events</para>
    /// <para>地图开始生成的事件</para>
    /// </summary>
    public static Action<MapGenerationStartEvent>? MapGenerationStartEvent;

    /// <summary>
    /// <para>Map generation completion event</para>
    /// <para>地图生成完成事件</para>
    /// </summary>
    public static Action<MapGenerationCompleteEvent>? MapGenerationCompleteEvent;
    
    /// <summary>
    /// <para>Player Instance Change Event</para>
    /// <para>玩家实例改变事件</para>
    /// </summary>
    public static Action<PlayerInstanceChangeEvent>? PlayerInstanceChangeEvent;
}