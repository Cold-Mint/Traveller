using ColdMint.scripts.map.dateBean;

namespace ColdMint.scripts.map.events;

public class MapGenerationPlaceRoomFinishEvent
{
    public string? RoomNodeDataId { get; set; }
    
    /// <summary>
    /// <para>RoomPlacementData</para>
    /// <para>房间的放置数据</para>
    /// </summary>
    public RoomPlacementData? RoomPlacementData { get; set; }
}