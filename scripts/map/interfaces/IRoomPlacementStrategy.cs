using System.Threading.Tasks;
using ColdMint.scripts.levelGraphEditor;
using ColdMint.scripts.map.dateBean;

namespace ColdMint.scripts.map.interfaces;

/// <summary>
/// <para>Room placement strategy</para>
/// <para>房间放置策略</para>
/// </summary>
public interface IRoomPlacementStrategy
{
    /// <summary>
    /// <para>Place the room in the designated location</para>
    /// <para>在指定的位置放置房间</para>
    /// </summary>
    /// <param name="roomPlacementData">
    ///<para>Room placement information</para>
    ///<para>房间放置信息</para>
    /// </param>
    /// <returns>
    ///<para>Placement success or not</para>
    ///<para>是否放置成功</para>
    /// </returns>
    public Task<bool> PlaceRoom(RoomPlacementData roomPlacementData);

    /// <summary>
    /// <para>Calculate new room placement information</para>
    /// <para>计算新的房间放置信息</para>
    /// </summary>
    /// <param name="parentRoomNode">
    ///<para>Parent room node</para>
    ///<para>父房间节点</para>
    /// </param>
    /// <param name="newRoomNodeData">
    ///<para>New room data to be placed</para>
    ///<para>欲放置的新房间数据</para>
    /// </param>
    /// <returns></returns>
    public Task<RoomPlacementData?> CalculateNewRoomPlacementData(IRoom? parentRoomNode, RoomNodeData newRoomNodeData);
}