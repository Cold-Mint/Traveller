using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColdMint.scripts.debug;
using ColdMint.scripts.levelGraphEditor;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.map.events;
using ColdMint.scripts.map.interfaces;
using ColdMint.scripts.map.LayoutParsingStrategy;
using ColdMint.scripts.map.layoutStrategy;
using ColdMint.scripts.map.room;
using ColdMint.scripts.serialization;
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

    private static bool _running;
    private static int _level = 1;

    /// <summary>
    /// <para>Room dictionary</para>
    /// <para>房间字典</para>
    /// </summary>
    private static readonly Dictionary<string, Room> RoomDictionary = new();

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

    private static Dictionary<string, IRoomInjectionProcessor>? _roomInjectionProcessorsDictionary;

    /// <summary>
    /// <para>GetRoomList</para>
    /// <para>获取房间列表</para>
    /// </summary>
    /// <returns></returns>
    public static string[] GetRoomList()
    {
        return RoomDictionary.Keys.ToArray();
    }

    /// <summary>
    /// <para>Get a room</para>
    /// <para>获取某个房间</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Room? GetRoom(string id)
    {
        return RoomDictionary.GetValueOrDefault(id);
    }

    /// <summary>
    /// <para>Register the room injection processor</para>
    /// <para>注册房间注入处理器</para>
    /// </summary>
    /// <param name="roomInjectionProcessor"></param>
    /// <returns></returns>
    public static bool RegisterRoomInjectionProcessor(IRoomInjectionProcessor roomInjectionProcessor)
    {
        var key = roomInjectionProcessor.GetId();
        if (_roomInjectionProcessorsDictionary == null)
        {
            _roomInjectionProcessorsDictionary = new Dictionary<string, IRoomInjectionProcessor>
            {
                {
                    key, roomInjectionProcessor
                }
            };
            return true;
        }

        return _roomInjectionProcessorsDictionary.TryAdd(key, roomInjectionProcessor);
    }

    /// <summary>
    /// <para>Log out of the room injection processor</para>
    /// <para>注销房间注入处理器</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool UnRegisterRoomInjectionProcessor(string id)
    {
        if (_roomInjectionProcessorsDictionary == null)
        {
            return false;
        }

        return _roomInjectionProcessorsDictionary.Remove(id);
    }

    /// <summary>
    /// <para>Level</para>
    /// <para>关卡等级</para>
    /// </summary>
    public static int Level
    {
        get => _level;
        set => _level = value;
    }

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
    /// <para>Clean up creatures and rooms from previous maps.</para>
    /// <para>清理先前地图上的生物和房间。</para>
    /// </summary>
    private static void CleanupPreviousMapEntities()
    {
        if (GameSceneDepend.AiCharacterContainer != null)
        {
            NodeUtils.DeleteAllChild(GameSceneDepend.AiCharacterContainer);
        }

        if (_mapRoot != null)
        {
            NodeUtils.DeleteAllChild(_mapRoot);
        }
    }

    /// <summary>
    /// <para>Generating a map</para>
    /// <para>生成地图</para>
    /// </summary>
    public static async Task GenerateMapAsync()
    {
        if (!CanStartGeneration())
        {
            StopMapGeneration(Config.MapGeneratorStopCode.BePrevented);
            return;
        }

        _running = true;
        EventBus.MapGenerationStartEvent?.Invoke(new MapGenerationStartEvent());
        RoomDictionary.Clear();
        CleanupPreviousMapEntities();
        if (!await InitializeGenerationAsync())
        {
            StopMapGeneration(Config.MapGeneratorStopCode.InitializationFailure);
            return;
        }

        if (_layoutStrategy == null || _layoutParsingStrategy == null)
        {
            StopMapGeneration(Config.MapGeneratorStopCode.ParameterIncompletenessDetected);
            return;
        }

        var levelGraphEditorSaveData = await _layoutStrategy.GetLayout(_level);
        if (levelGraphEditorSaveData == null || !IsValidLayoutData(levelGraphEditorSaveData))
        {
            StopMapGeneration(Config.MapGeneratorStopCode.LevelGraphIsNotAvailable);
            return;
        }

        _layoutParsingStrategy.SetLevelGraph(levelGraphEditorSaveData);
        var randomNumberGenerator = new RandomNumberGenerator
        {
            Seed = _seed
        };
        if (!await ProcessStartingRoomAsync(randomNumberGenerator, RoomDictionary))
        {
            StopMapGeneration(Config.MapGeneratorStopCode.InitialRoomPlacementFailed);
            return;
        }

        while (await _layoutParsingStrategy.HasNext())
        {
            await ProcessNextRoomAsync(randomNumberGenerator, RoomDictionary);
        }

        FinalizeMapGeneration(RoomDictionary, randomNumberGenerator);
    }

    /// <summary>
    /// <para>CanStartGeneration</para>
    /// <para>是否可以开始生成</para>
    /// </summary>
    /// <returns></returns>
    private static bool CanStartGeneration()
    {
        if (_running)
        {
            LogCat.LogWarning("map_generator_is_running");
            return false;
        }

        if (_layoutStrategy == null || _roomPlacementStrategy == null || _layoutParsingStrategy == null ||
            _mapRoot == null)
        {
            LogCat.LogError("map_generator_missing_parameters");
            return false;
        }

        return true;
    }

    /// <summary>
    /// <para>Initialize Generation</para>
    /// <para>初始化生成器</para>
    /// </summary>
    /// <returns></returns>
    private static async Task<bool> InitializeGenerationAsync()
    {
        if (_roomPlacementStrategy == null || _mapRoot == null)
        {
            return false;
        }

        if (!await _roomPlacementStrategy.StartGeneration(_mapRoot))
        {
            LogCat.LogError("room_placement_strategy_terminates_map_generation");
            return false;
        }

        return true;
    }

    /// <summary>
    /// <para>IsValidLayoutData</para>
    /// <para>是否有效的布局数据</para>
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static bool IsValidLayoutData(LevelGraphEditorSaveData? data)
    {
        if (data?.RoomNodeDataList == null || data.RoomNodeDataList.Count == 0)
        {
            LogCat.LogError("map_generator_attempts_to_parse_empty_layout_diagrams");
            return false;
        }

        return true;
    }

    /// <summary>
    /// <para>Process Starting Room</para>
    /// <para>处理起始房间</para>
    /// </summary>
    /// <param name="randomNumberGenerator"></param>
    /// <param name="roomDictionary"></param>
    /// <returns></returns>
    private static async Task<bool> ProcessStartingRoomAsync(RandomNumberGenerator randomNumberGenerator,
        Dictionary<string, Room> roomDictionary)
    {
        if (_layoutParsingStrategy == null || _roomPlacementStrategy == null) return false;
        var startRoomNodeData = await _layoutParsingStrategy.GetStartRoomNodeData();
        if (startRoomNodeData == null || string.IsNullOrEmpty(startRoomNodeData.Id))
        {
            LogCat.LogError("map_generator_has_no_starting_room_data");
            return false;
        }

        var startingRoomPlacementData = await _roomPlacementStrategy.CalculatePlacementDataForStartingRoom(
            randomNumberGenerator, startRoomNodeData);
        if (startingRoomPlacementData == null ||
            !await PlaceRoomAndAddRecordAsync(startRoomNodeData.Id, startingRoomPlacementData, roomDictionary))
        {
            LogCat.LogError("start_room_placement_failed");
            return false;
        }

        return true;
    }

    /// <summary>
    /// <para>Take care of the next room</para>
    /// <para>处理下一个房间</para>
    /// </summary>
    /// <param name="randomNumberGenerator"></param>
    /// <param name="roomDictionary"></param>
    private static async Task ProcessNextRoomAsync(RandomNumberGenerator randomNumberGenerator,
        Dictionary<string, Room> roomDictionary)
    {
        if (_layoutParsingStrategy == null || _roomPlacementStrategy == null)
        {
            return;
        }

        var roomNodeData = await _layoutParsingStrategy.Next();
        if (roomNodeData == null || string.IsNullOrEmpty(roomNodeData.Id))
        {
            LogCat.LogWarning("room_data_missing");
            return;
        }

        if (await CanPlaceRoomAsync(randomNumberGenerator, roomNodeData.RoomInjectionProcessorData))
        {
            var nextParentNodeId = await _layoutParsingStrategy.GetNextParentNodeId();
            Room? parentRoomNode = null;
            if (!string.IsNullOrEmpty(nextParentNodeId) && roomDictionary.TryGetValue(nextParentNodeId, out var value))
            {
                parentRoomNode = value;
            }

            var roomPlacementData =
                await _roomPlacementStrategy.CalculateNewRoomPlacementData(randomNumberGenerator, parentRoomNode,
                    roomNodeData);
            if (roomPlacementData != null &&
                await PlaceRoomAndAddRecordAsync(roomNodeData.Id, roomPlacementData, roomDictionary))
            {
                MarkRoomSlot(roomPlacementData);
            }
            else
            {
                LogCat.LogWithFormat("failed_to_calculate_the_room_location", LogCat.LogLabel.Default, roomNodeData.Id);
            }
        }
    }

    /// <summary>
    /// <para>CanPlaceRoom</para>
    /// <para>是否可以放置房间</para>
    /// </summary>
    /// <param name="randomNumberGenerator"></param>
    /// <param name="roomInjectionProcessorData"></param>
    /// <returns></returns>
    private static async Task<bool> CanPlaceRoomAsync(RandomNumberGenerator randomNumberGenerator,
        string? roomInjectionProcessorData)
    {
        if (_roomInjectionProcessorsDictionary != null && !string.IsNullOrEmpty(roomInjectionProcessorData))
        {
            var roomInjectionProcessorDataArray =
                YamlSerialization.Deserialize<RoomInjectionProcessorData[]>(roomInjectionProcessorData);
            if (roomInjectionProcessorDataArray is { Length: > 0 })
            {
                foreach (var injectionProcessorData in roomInjectionProcessorDataArray)
                {
                    if (string.IsNullOrEmpty(injectionProcessorData.Id) ||
                        string.IsNullOrEmpty(injectionProcessorData.Config))
                        continue;

                    if (!_roomInjectionProcessorsDictionary.TryGetValue(injectionProcessorData.Id,
                            out var roomInjectionProcessor))
                    {
                        LogCat.LogErrorWithFormat("room_injection_processor_does_not_exist", LogCat.LogLabel.Default,
                            injectionProcessorData.Id);
                        continue;
                    }

                    if (!await roomInjectionProcessor.CanBePlaced(randomNumberGenerator, injectionProcessorData.Config))
                        return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// <para>Stop map generation</para>
    /// <para>停止地图生成</para>
    /// </summary>
    private static void StopMapGeneration(int code)
    {
        if (code != Config.MapGeneratorStopCode.Normal)
        {
            LogCat.LogErrorWithFormat("map_generator_error", LogCat.LogLabel.Default, code);
        }

        _running = false;
    }

    /// <summary>
    /// <para>FinalizeMapGeneration</para>
    /// <para>完成地图生成</para>
    /// </summary>
    /// <param name="roomDictionary"></param>
    /// <param name="randomNumberGenerator"></param>
    private static void FinalizeMapGeneration(Dictionary<string, Room> roomDictionary,
        RandomNumberGenerator randomNumberGenerator)
    {
        foreach (var room in roomDictionary.Values)
        {
            PlaceBarrier(room);
        }

        if (_roomPlacementStrategy != null && _mapRoot != null)
        {
            _roomPlacementStrategy.GeneratedComplete(_mapRoot);
        }

        StopMapGeneration(Config.MapGeneratorStopCode.Normal);
        EventBus.MapGenerationCompleteEvent?.Invoke(new MapGenerationCompleteEvent
        {
            RandomNumberGenerator = randomNumberGenerator,
            RoomDictionary = roomDictionary
        });
    }


    /// <summary>
    /// <para>Place barriers</para>
    /// <para>放置屏障</para>
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    private static void PlaceBarrier(Room? room)
    {
        if (room == null)
        {
            return;
        }

        room.PlaceBarrierInUnmatchedSlots();
    }

    /// <summary>
    /// <para>Mark the room slot as matched</para>
    /// <para>将房间槽标记为已匹配</para>
    /// </summary>
    /// <param name="roomPlacementData"></param>
    private static void MarkRoomSlot(RoomPlacementData roomPlacementData)
    {
        if (roomPlacementData.ParentRoomSlot != null)
        {
            roomPlacementData.ParentRoomSlot.Matched = true;
        }

        if (roomPlacementData.NewRoomSlot != null)
        {
            roomPlacementData.NewRoomSlot.Matched = true;
        }
    }

    /// <summary>
    /// <para>Place rooms and add mappings</para>
    /// <para>放置房间，并增加映射</para>
    /// </summary>
    /// <param name="roomNodeDataId"></param>
    /// <param name="roomPlacementData"></param>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    private static async Task<bool> PlaceRoomAndAddRecordAsync(string roomNodeDataId,
        RoomPlacementData roomPlacementData, Dictionary<string, Room> dictionary)
    {
        //The input parameters are incomplete.
        //输入参数不全。
        if (_roomPlacementStrategy == null || _mapRoot == null || string.IsNullOrEmpty(roomNodeDataId) ||
            roomPlacementData.NewRoom == null)
        {
            return false;
        }

        if (dictionary.ContainsKey(roomNodeDataId))
        {
            LogCat.LogWithFormat("place_existing_rooms", LogCat.LogLabel.Default, roomNodeDataId);
            return false;
        }

        if (!await _roomPlacementStrategy.PlaceRoom(_mapRoot, roomPlacementData))
        {
            LogCat.LogWarningWithFormat("room_placement_failed", LogCat.LogLabel.Default,
                roomNodeDataId);
            return false;
        }

        dictionary.Add(roomNodeDataId, roomPlacementData.NewRoom);
        LogCat.LogWithFormat("room_placement_information", LogCat.LogLabel.Default, roomNodeDataId,
            roomPlacementData.Position.ToString());
        return true;
    }
}