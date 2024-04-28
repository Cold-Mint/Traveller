namespace ColdMint.scripts.map.interfaces;

public interface IRoomHolder
{
    bool AddRoom(IRoom room);
    
    /// <summary>
    /// <para>LastRoom</para>
    /// <para>最后添加的房间</para>
    /// </summary>
    IRoom LastRoom { get; }
    
    /// <summary>
    /// <para>Number of rooms that have been placed</para>
    /// <para>已放置的房间数量</para>
    /// </summary>
    int PlacedRoomNumber { get; }
}