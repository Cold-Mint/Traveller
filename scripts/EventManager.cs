using System;
using ColdMint.scripts.map.events;

namespace ColdMint.scripts;

public class EventManager
{
    /// <summary>
    /// <para>Event when the AI character is generated</para>
    /// <para>AI角色生成事件</para>
    /// </summary>
    public static Action<AiCharacterGenerateEvent>? AiCharacterGenerateEvent;

    public static Action<GameOverEvent>? GameOverEvent;

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
}