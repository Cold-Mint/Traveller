using System.Threading.Tasks;
using ColdMint.scripts.levelGraphEditor;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.map.interfaces;
using Godot;

namespace ColdMint.scripts.map.RoomPlacer;

/// <summary>
/// <para>Patchwork room placement strategy</para>
/// <para>拼接的房间放置策略</para>
/// </summary>
/// <remarks>
///<para>Under this strategy, think of each room template as a puzzle piece, find their "slots", and then connect them together.</para>
///<para>在此策略下，将每个房间模板看作是一块拼图，找到他们的“槽”，然后将其连接在一起。</para>
/// </remarks>
public class PatchworkRoomPlacementStrategy : IRoomPlacementStrategy
{
    public Task<bool> PlaceRoom(RoomPlacementData roomPlacementData)
    {
        throw new System.NotImplementedException();
    }

    public Task<RoomPlacementData?> GetStartRoomPlacementData(RoomNodeData startRoomNodeData)
    {
        throw new System.NotImplementedException();
    }


    public Task<RoomPlacementData?> CalculateNewRoomPlacementData(IRoom parentRoomNode, RoomNodeData newRoomNodeData)
    {
        throw new System.NotImplementedException();
    }
}