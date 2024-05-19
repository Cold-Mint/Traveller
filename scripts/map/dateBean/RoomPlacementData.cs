using ColdMint.scripts.map.interfaces;
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
    public IRoom? Room { get; set; }
}