using ColdMint.scripts.map.room;

namespace ColdMint.scripts.map.interfaces;

/// <summary>
/// <para>Room holder</para>
/// <para>房间持有者</para>
/// </summary>
/// <remarks>
///<para>This class holds all generated and placed rooms.</para>
///<para>该类保存所有已生成且已放置的房间。</para>
/// </remarks>
public interface IRoomHolder
{
    bool AddRoom(Room room);
    
    /// <summary>
    /// <para>LastRoom</para>
    /// <para>最后添加的房间</para>
    /// </summary>
    Room? LastRoom { get; }
    
    /// <summary>
    /// <para>Number of rooms that have been placed</para>
    /// <para>已放置的房间数量</para>
    /// </summary>
    int PlacedRoomNumber { get; }
}