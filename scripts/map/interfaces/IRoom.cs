using ColdMint.scripts.map.dateBean;
using Godot;

namespace ColdMint.scripts.map.interfaces;

/// <summary>
/// <para>IRoom</para>
/// <para>表示房间</para>
/// </summary>
public interface IRoom
{
    /// <summary>
    /// <para>Set room scene</para>
    /// <para>设置房间场景</para>
    /// </summary>
    PackedScene? RoomScene { get; set; }
    
    /// <summary>
    /// <para>Tile map</para>
    /// <para>瓦片地图</para>
    /// </summary>
    TileMap? TileMap { get; set; }

    /// <summary>
    /// <para>Gets the root node of the room</para>
    /// <para>获取房间的根节点</para>
    /// </summary>
    Node2D? RootNode { get; }

    /// <summary>
    /// <para>The room holds the corresponding slot data</para>
    /// <para>房间持有对应的插槽数据</para>
    /// </summary>
    RoomSlot?[]? RoomSlots { get; }
}