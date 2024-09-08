using ColdMint.scripts.map.dateBean;

namespace ColdMint.scripts.map.events;

/// <summary>
/// <para>MapGenerationPlaceRoomFinishEvent</para>
/// <para>地图生成器放置房间完成后执行的事件</para>
/// </summary>
public class MapGenerationPlaceRoomFinishEvent
{
    public string? RoomNodeDataId { get; set; }
    
    /// <summary>
    /// <para>RoomPlacementData</para>
    /// <para>房间的放置数据</para>
    /// </summary>
    public RoomPlacementData? RoomPlacementData { get; set; }
}