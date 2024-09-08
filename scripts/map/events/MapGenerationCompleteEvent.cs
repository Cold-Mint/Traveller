
using System.Collections.Generic;
using ColdMint.scripts.map.room;
using Godot;

namespace ColdMint.scripts.map.events;

/// <summary>
/// <para>Event when the map is created</para>
/// <para>地图创建完成的事件</para>
/// </summary>
public class MapGenerationCompleteEvent
{
    /// <summary>
    /// <para>Random number generator generated from seed</para>
    /// <para>根据种子生成的随机数生成器</para>
    /// </summary>
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public RandomNumberGenerator? RandomNumberGenerator { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global
    
    /// <summary>
    /// <para>All placed rooms</para>
    /// <para>所有已放置的房间</para>
    /// </summary>
    public Dictionary<string, Room>? RoomDictionary { get; set; }
}