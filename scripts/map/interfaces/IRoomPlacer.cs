using System.Threading.Tasks;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.map.RoomPlacer;
using Godot;

namespace ColdMint.scripts.map.interfaces;

/// <summary>
/// <para>Room placer</para>
/// <para>房间放置器</para>
/// </summary>
/// <remarks>
///<para>Responsible for arranging the rooms on the map</para>
///<para>负责在地图中摆放房间</para>
/// </remarks>
public interface IRoomPlacer
{

    /// <summary>
    /// <para>Place the room in the designated location</para>
    /// <para>在指定的位置放置房间</para>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="room"></param>
    /// <returns></returns>
    public Task<bool> PlaceRoom(Vector2 position, IRoom room);

    /// <summary>
    /// <para>Pass into two rooms and calculate the location of the new room</para>
    /// <para>传入两个房间，计算新房间的位置</para>
    /// </summary>
    /// <param name="mainRoom"></param>
    /// <param name="newRoom"></param>
    /// <returns></returns>
    public Task<Vector2> CalculatedPosition(IRoom mainRoom, IRoom newRoom, RoomSlot mainRoomSlot, RoomSlot newRoomSlot,
        RoomPlacerConfig roomPlacerConfig);
}