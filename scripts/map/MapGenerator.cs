using System.Collections.Generic;
using System.Threading.Tasks;
using ColdMint.scripts.debug;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.map.interfaces;
using ColdMint.scripts.map.LayoutParsingStrategy;
using ColdMint.scripts.map.layoutStrategy;
using ColdMint.scripts.map.room;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.map;

/// <summary>
/// <para>Map generator</para>
/// <para>地图生成器</para>
/// </summary>
/// <remarks>
///<para>Responsible for the overall map generation process control</para>
///<para>负责地图的整体生成流程控制</para>
/// </remarks>
public static class MapGenerator
{
    /// <summary>
    /// <para>Layout map selection strategy</para>
    /// <para>布局图选择策略</para>
    /// </summary>
    private static ILayoutStrategy? _layoutStrategy;

    /// <summary>
    /// <para>Map root node</para>
    /// <para>地图根节点</para>
    /// </summary>
    private static Node? _mapRoot;

    /// <summary>
    /// <para>Room placement strategy</para>
    /// <para>房间的放置策略</para>
    /// </summary>
    private static IRoomPlacementStrategy? _roomPlacementStrategy;

    private static ulong _seed;

    /// <summary>
    /// <para>Set seed</para>
    /// <para>设置种子</para>
    /// </summary>
    public static string Seed
    {
        get => _seed.ToString();
        //If the player inputs integers, we seed them directly with the input values. If it is not an integer, the hash value is taken.
        //如果玩家输入的是整数，那么我们直接用输入值作为种子。如果不是整数，则取哈希值。
        set => _seed = ulong.TryParse(value, out var result) ? result : HashCodeUtils.GetFixedHashCode(value);
    }

    /// <summary>
    /// <para>Layout diagram parsing policy</para>
    /// <para>布局图解析策略</para>
    /// </summary>
    private static ILayoutParsingStrategy? _layoutParsingStrategy;

    public static Node? MapRoot
    {
        get => _mapRoot;
        set => _mapRoot = value;
    }

    public static ILayoutParsingStrategy? LayoutParsingStrategy
    {
        get => _layoutParsingStrategy;
        set => _layoutParsingStrategy = value;
    }

    public static IRoomPlacementStrategy? RoomPlacementStrategy
    {
        get => _roomPlacementStrategy;
        set => _roomPlacementStrategy = value;
    }

    public static ILayoutStrategy? LayoutStrategy
    {
        get => _layoutStrategy;
        set => _layoutStrategy = value;
    }

    /// <summary>
    /// <para>Generating a map</para>
    /// <para>生成地图</para>
    /// </summary>
    public static async Task GenerateMap()
    {
        if (_layoutStrategy == null || _roomPlacementStrategy == null || _layoutParsingStrategy == null ||
            _mapRoot == null)
        {
            LogCat.LogError("map_generator_missing_parameters");
            return;
        }

        NodeUtils.DeleteAllChild(_mapRoot);
        if (!await _roomPlacementStrategy.StartGeneration(_mapRoot))
        {
            return;
        }

        //Get the layout data
        //拿到布局图数据
        var levelGraphEditorSaveData = await _layoutStrategy.GetLayout();
        if (levelGraphEditorSaveData == null || levelGraphEditorSaveData.RoomNodeDataList == null ||
            levelGraphEditorSaveData.RoomNodeDataList.Count == 0)
        {
            LogCat.LogError("map_generator_attempts_to_parse_empty_layout_diagrams");
            return;
        }

        _layoutParsingStrategy.SetLevelGraph(levelGraphEditorSaveData);
        //Save the dictionary, put the ID in the room data, corresponding to the successful placement of the room.
        //保存字典，将房间数据内的ID，对应放置成功的房间。
        var roomDictionary = new Dictionary<string, Room>();
        var randomNumberGenerator = new RandomNumberGenerator();
        randomNumberGenerator.Seed = _seed;
        LogCat.LogWithFormat("seed_info", _seed);
        var startRoomNodeData = await _layoutParsingStrategy.GetStartRoomNodeData();
        if (startRoomNodeData == null || string.IsNullOrEmpty(startRoomNodeData.Id))
        {
            LogCat.LogError("map_generator_has_no_starting_room_data");
            return;
        }

        var startingRoomPlacementData =
            await _roomPlacementStrategy.CalculatePlacementDataForStartingRoom(randomNumberGenerator,
                startRoomNodeData);
        if (startingRoomPlacementData == null)
        {
            return;
        }

        var placeSuccess = await PlaceRoomAndAddRecord(startRoomNodeData.Id, startingRoomPlacementData, roomDictionary);
        if (!placeSuccess)
        {
            return;
        }

        while (await _layoutParsingStrategy.HasNext())
        {
            //When a new room needs to be placed
            //当有新的房间需要放置时
            var roomNodeData = await _layoutParsingStrategy.Next();
            if (roomNodeData == null || string.IsNullOrEmpty(roomNodeData.Id))
            {
                continue;
            }

            var nextParentNodeId = await _layoutParsingStrategy.GetNextParentNodeId();
            Room? parentRoomNode = null;
            if (nextParentNodeId != null)
            {
                //If the new room has the parent's ID, then we pass the parent's room into the compute function.
                //如果新房间有父节点的ID，那么我们将父节点的房间传入到计算函数内。
                if (roomDictionary.TryGetValue(nextParentNodeId, out var value))
                {
                    parentRoomNode = value;
                }
            }

            var roomPlacementData =
                await _roomPlacementStrategy.CalculateNewRoomPlacementData(randomNumberGenerator, parentRoomNode,
                    roomNodeData);
            if (roomPlacementData == null)
            {
                continue;
            }

            await PlaceRoomAndAddRecord(roomNodeData.Id, roomPlacementData, roomDictionary);
        }
        //All rooms have been placed.
        //所有房间已放置完毕。
        await _roomPlacementStrategy.GeneratedComplete(_mapRoot);
    }

    /// <summary>
    /// <para>Place rooms and add mappings</para>
    /// <para>放置房间，并增加映射</para>
    /// </summary>
    /// <param name="roomNodeDataId"></param>
    /// <param name="roomPlacementData"></param>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    private static async Task<bool> PlaceRoomAndAddRecord(string roomNodeDataId,
        RoomPlacementData roomPlacementData, Dictionary<string, Room> dictionary)
    {
        //The input parameters are incomplete.
        //输入参数不全。
        if (_roomPlacementStrategy == null || _mapRoot == null || string.IsNullOrEmpty(roomNodeDataId) ||
            roomPlacementData.Room == null)
        {
            return false;
        }

        if (dictionary.ContainsKey(roomNodeDataId))
        {
            return false;
        }

        if (!await _roomPlacementStrategy.PlaceRoom(_mapRoot, roomPlacementData))
        {
            return false;
        }

        dictionary.Add(roomNodeDataId, roomPlacementData.Room);
        return true;
    }
}