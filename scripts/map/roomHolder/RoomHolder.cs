using System.Collections.Generic;
using ColdMint.scripts.map.interfaces;
using ColdMint.scripts.map.room;

namespace ColdMint.scripts.map.roomHolder;

public class RoomHolder : IRoomHolder
{
    private readonly List<Room> _rooms = new List<Room>();

    public bool AddRoom(Room room)
    {
        _rooms.Add(room);
        return true;
    }

    public Room? LastRoom
    {
        get
        {
            if (_rooms.Count > 0)
            {
                return _rooms[_rooms.Count - 1];
            }

            return null;
        }
    }

    public int PlacedRoomNumber => _rooms.Count;
}