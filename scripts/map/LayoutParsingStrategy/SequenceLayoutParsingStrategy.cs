using System.Collections.Generic;
using System.Linq;
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


    //The connection index of the query
    //查询的连接索引
    private int _index;
    private int _maxIndex;
    private Dictionary<string, RoomNodeData> _roomNodeDataDictionary = new Dictionary<string, RoomNodeData>();

    public void SetLevelGraph(LevelGraphEditorSaveData levelGraphEditorSaveData)
    {
        _checkLegality = false;
        _index = -1;
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
            _maxIndex = _levelGraphEditorSaveData.ConnectionDataList.Count;
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
        _checkLegality = true;
    }

    public Task<RoomNodeData?> GetStartRoomNodeData()
    {
        if (_levelGraphEditorSaveData == null)
        {
            return Task.FromResult<RoomNodeData?>(null);
        }

        if (_levelGraphEditorSaveData.RoomNodeDataList == null || _levelGraphEditorSaveData.RoomNodeDataList.Count == 0)
        {
            //If there is no room data in the level map set.
            //如果设置的关卡图内没有房间数据。
            return Task.FromResult<RoomNodeData?>(null);
        }

        foreach (var roomNodeData in _levelGraphEditorSaveData.RoomNodeDataList.Where(roomNodeData =>
                     roomNodeData.HasTag(Config.RoomDataTag.StartingRoom)))
        {
            return Task.FromResult<RoomNodeData?>(roomNodeData);
        }

        return Task.FromResult<RoomNodeData?>(null);
    }

    public Task<RoomNodeData?> Next()
    {
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

        _index++;
        return Task.FromResult(_index < _maxIndex);
    }
}