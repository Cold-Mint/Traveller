using ColdMint.scripts.map.room;
using Godot;

namespace ColdMint.scripts.map.dateBean;

/// <summary>
/// <para>Room placement information</para>
/// <para>房间放置信息</para>
/// </summary>
public class RoomPlacementData
{
    /// <summary>
    /// <para>the location of placement</para>
    /// <para>放置的位置</para>
    /// </summary>
    public Vector2? Position { get; set; }

    /// <summary>
    /// <para>Place the room template</para>
    /// <para>放置的房间模板</para>
    /// </summary>
    public Room? NewRoom { get; set; }
    
    /// <summary>
    /// <para>Parent room</para>
    /// <para>父级房间</para>
    /// </summary>
    public Room? ParentRoom { get; set; }

    /// <summary>
    /// <para>Parent room slot</para>
    /// <para>父级房间的插槽</para>
    /// </summary>
    public RoomSlot? ParentRoomSlot { get; set; }

    /// <summary>
    /// <para>A slot for the new room</para>
    /// <para>新房间的插槽</para>
    /// </summary>
    public RoomSlot? NewRoomSlot { get; set; }
}