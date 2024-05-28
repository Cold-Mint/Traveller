
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
    public RandomNumberGenerator? RandomNumberGenerator { get; set; }
}