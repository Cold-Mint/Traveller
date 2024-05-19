using System.Collections.Generic;
using System.Threading.Tasks;
using ColdMint.scripts.levelGraphEditor;

namespace ColdMint.scripts.map.LayoutParsingStrategy;

/// <summary>
/// <para>Sequential layout diagram parsing strategy</para>
/// <para>顺序的布局图解析策略</para>
/// </summary>
/// <remarks>
///<para>Traverse from the start element to the end element according to the ConnectionDataList. This policy considers the first element of the ConnectionDataList to be the fixed starting room information.</para>
///<para>按照ConnectionDataList从起始元素遍历到末项元素。本策略认为ConnectionDataList的首个元素为固定的起始房间信息。</para>
/// <para></para>
/// <para>This policy's Next() method does not return rooms that are not connected except for the start room</para>
/// <para>本策略Next()方法不会返回除了起始房间外，没有连接的房间</para>
/// </remarks>
public class SequenceLayoutParsingStrategy : ILayoutParsingStrategy
{
    private LevelGraphEditorSaveData? _levelGraphEditorSaveData;

    //Check whether data Settings are valid
    //设置数据时，是否检查合法了
    private bool _checkLegality;

    //A special marker for the starting room.
    //特殊标记，代表起始房间。
    private const int StartingRoomIndex = -1;

    //The connection index of the query
    //查询的连接索引
    private int _index = StartingRoomIndex;
    private int _maxIndex;
    private Dictionary<string, RoomNodeData> _roomNodeDataDictionary = new Dictionary<string, RoomNodeData>();

    public void SetLevelGraph(LevelGraphEditorSaveData levelGraphEditorSaveData)
    {
        _index = StartingRoomIndex;
        _levelGraphEditorSaveData = levelGraphEditorSaveData;
        if (_levelGraphEditorSaveData.RoomNodeDataList == null || _levelGraphEditorSaveData.RoomNodeDataList.Count == 0)
        {
            //No room data, unable to parse.
            //没有房间数据，无法解析。
            return;
        }

        if (_levelGraphEditorSaveData.ConnectionDataList == null ||
            _levelGraphEditorSaveData.ConnectionDataList.Count == 0)
        {
            _maxIndex = 0;
        }
        else
        {
            _maxIndex = _levelGraphEditorSaveData.ConnectionDataList.Count - 1;
        }

        _roomNodeDataDictionary.Clear();
        foreach (var roomNodeData in _levelGraphEditorSaveData.RoomNodeDataList)
        {
            if (roomNodeData.Id == null)
            {
                continue;
            }

            _roomNodeDataDictionary.Add(roomNodeData.Id, roomNodeData);
        }

        //Check that the first room is the starting room.
        //检查首个房间是否为起始房间。
        var firstRoom = GetFirstRoom(_levelGraphEditorSaveData);
        if (firstRoom == null)
        {
            return;
        }

        _checkLegality = firstRoom.HasTag(Config.RoomDataTag.StartingRoom);
    }

    /// <summary>
    /// <para>Get the first room</para>
    /// <para>获取第一个房间</para>
    /// </summary>
    /// <param name="levelGraphEditorSaveData"></param>
    /// <returns></returns>
    private RoomNodeData? GetFirstRoom(LevelGraphEditorSaveData? levelGraphEditorSaveData)
    {
        if (levelGraphEditorSaveData == null || levelGraphEditorSaveData.RoomNodeDataList == null ||
            levelGraphEditorSaveData.RoomNodeDataList.Count == 0)
        {
            return null;
        }

        RoomNodeData? firstRoom = null;
        if (levelGraphEditorSaveData.ConnectionDataList == null ||
            levelGraphEditorSaveData.ConnectionDataList.Count == 0)
        {
            //If there is no connection information, then fetch the first room in the RoomNodeDataList.
            //如果没有连接信息，那么在RoomNodeDataList内取出第一个房间。
            firstRoom = levelGraphEditorSaveData.RoomNodeDataList[0];
        }
        else
        {
            //If there is connection information, then fetch the first connected From room in the ConnectionDataList.
            //如果有连接信息，那么在ConnectionDataList内取出第一个连接的From房间。
            var firstConnection = levelGraphEditorSaveData.ConnectionDataList[0];
            if (firstConnection.FromId == null)
            {
                return firstRoom;
            }

            if (_roomNodeDataDictionary.TryGetValue(firstConnection.FromId, out var value))
            {
                firstRoom = value;
            }
        }

        return firstRoom;
    }

    public Task<RoomNodeData?> Next()
    {
        if (_index == StartingRoomIndex)
        {
            return Task.FromResult(GetFirstRoom(_levelGraphEditorSaveData));
        }

        var connectionData = GetIndexOfConnectionData(_index);
        if (connectionData == null)
        {
            return Task.FromResult<RoomNodeData?>(null);
        }

        if (connectionData.ToId == null)
        {
            return Task.FromResult<RoomNodeData?>(null);
        }

        if (_roomNodeDataDictionary.TryGetValue(connectionData.ToId, out var value))
        {
            return Task.FromResult<RoomNodeData?>(value);
        }

        return Task.FromResult<RoomNodeData?>(null);
    }

    /// <summary>
    /// <para>Gets connection information for the specified location</para>
    /// <para>获取指定位置的连接信息</para>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private ConnectionData? GetIndexOfConnectionData(int index)
    {
        if (_levelGraphEditorSaveData == null)
        {
            return null;
        }

        if (_levelGraphEditorSaveData.ConnectionDataList == null ||
            _levelGraphEditorSaveData.ConnectionDataList.Count == 0)
        {
            return null;
        }

        return _levelGraphEditorSaveData.ConnectionDataList[index];
    }

    public Task<string?> GetNextParentNodeId()
    {
        if (_index == StartingRoomIndex)
        {
            //The start room will not have a parent node.
            //起始房间不会有父节点。
            return Task.FromResult<string?>(null);
        }

        var connectionData = GetIndexOfConnectionData(_index);
        if (connectionData == null)
        {
            return Task.FromResult<string?>(null);
        }

        if (connectionData.FromId == null)
        {
            return Task.FromResult<string?>(null);
        }

        if (_roomNodeDataDictionary.ContainsKey(connectionData.FromId))
        {
            return Task.FromResult<string?>(connectionData.FromId);
        }

        return Task.FromResult<string?>(null);
    }

    public Task<bool> HasNext()
    {
        if (!_checkLegality)
        {
            //If the check is not valid, then simply return false
            //如果检查不合法，那么直接返回false
            return Task.FromResult(false);
        }

        if (_index == StartingRoomIndex)
        {
            //The start room is always considered to have the next room, in order to handle situations where levelGraphEditorSaveData has only room data and no connection data.
            //起始房间始终被认为有下一个房间，这是为了处理levelGraphEditorSaveData仅有房间数据，没有连接数据的情况。
            _index++;
            return Task.FromResult(true);
        }

        return Task.FromResult(_index < _maxIndex);
    }
}