using System.Threading.Tasks;
using ColdMint.scripts.levelGraphEditor;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.map.room;
using Godot;

namespace ColdMint.scripts.map.interfaces;

/// <summary>
/// <para>Room placement strategy</para>
/// <para>房间放置策略</para>
/// </summary>
public interface IRoomPlacementStrategy
{
    /// <summary>
    /// <para>This method is called when the map generator starts generating the map</para>
    /// <para>当地图生成器开始生成地图时调用此方法</para>
    /// </summary>
    /// <param name="mapRoot"></param>
    /// <returns>
    ///<para>Returning false terminates the build</para>
    ///<para>返回false则终止生成</para>
    /// </returns>
    public Task<bool> StartGeneration(Node mapRoot);

    /// <summary>
    /// <para>Call this method after the build is complete</para>
    /// <para>生成完成后调用此方法</para>
    /// </summary>
    /// <returns></returns>
    public Task GeneratedComplete(Node mapRoot);
    
    ///  <summary>
    ///  <para>Place the room in the designated location</para>
    ///  <para>在指定的位置放置房间</para>
    ///  </summary>
    ///  <param name="mapRoot"></param>
    ///  <param name="roomPlacementData">
    /// <para>Room placement information</para>
    /// <para>房间放置信息</para>
    ///  </param>
    ///  <returns>
    /// <para>Placement success or not</para>
    /// <para>是否放置成功</para>
    ///  </returns>
    public Task<bool> PlaceRoom(Node mapRoot, RoomPlacementData roomPlacementData);

    ///  <summary>
    ///  <para>Calculate new room placement information</para>
    ///  <para>计算新的房间放置信息</para>
    ///  </summary>
    ///  <param name="randomNumberGenerator">
    ///<para>randomNumberGenerator</para>
    ///<para>随机数生成器</para>
    /// </param>
    ///  <param name="parentRoomNode">
    /// <para>Parent room node</para>
    /// <para>父房间节点</para>
    ///  </param>
    ///  <param name="newRoomNodeData">
    /// <para>New room data to be placed</para>
    /// <para>欲放置的新房间数据</para>
    ///  </param>
    ///  <returns></returns>
    public Task<RoomPlacementData?> CalculateNewRoomPlacementData(RandomNumberGenerator randomNumberGenerator,
        Room? parentRoomNode,
        RoomNodeData newRoomNodeData);


    /// <summary>
    /// <para>Calculates the placement information for the starting room</para>
    /// <para>计算起始房间的放置信息</para>
    /// </summary>
    /// <param name="randomNumberGenerator"></param>
    /// <param name="startRoomNodeData"></param>
    /// <returns></returns>
    public Task<RoomPlacementData?> CalculatePlacementDataForStartingRoom(
        RandomNumberGenerator randomNumberGenerator, RoomNodeData startRoomNodeData);
}