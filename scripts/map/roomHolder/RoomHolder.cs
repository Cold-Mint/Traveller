using System.Collections.Generic;
using ColdMint.scripts.map.interfaces;

namespace ColdMint.scripts.map.roomHolder;

public class RoomHolder : IRoomHolder
{
    private readonly List<IRoom> _rooms = new List<IRoom>();

    public bool AddRoom(IRoom room)
    {
        _rooms.Add(room);
        return true;
    }

    public IRoom LastRoom
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