using System;
using ColdMint.scripts.map.events;

namespace ColdMint.scripts;

/// <summary>
/// <para>EventBus</para>
/// <para>事件总线</para>
/// </summary>
public static class EventBus
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
    /// <para>New rooms are placed within the map to perform events</para>
    /// <para>新的房间被放置在地图内执行的事件</para>
    /// </summary>
    public static Action<MapGenerationPlaceRoomFinishEvent>? MapGenerationPlaceRoomFinishEvent;

    /// <summary>
    /// <para>Map generation completion event</para>
    /// <para>地图生成完成事件</para>
    /// </summary>
    public static Action<MapGenerationCompleteEvent>? MapGenerationCompleteEvent;
}