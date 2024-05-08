using System.Threading.Tasks;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.map.interfaces;
using Godot;

namespace ColdMint.scripts.map.RoomPlacer;

public abstract class RoomPlacerTemplate : IRoomPlacer
{
    public abstract Task<bool> PlaceRoom(Vector2 position, IRoom room);

    public abstract Task<Vector2> CalculatedPosition(IRoom mainRoom, IRoom newRoom, RoomSlot? mainRoomSlot, RoomSlot? newRoomSlot,
        RoomPlacerConfig roomPlacerConfig);
}