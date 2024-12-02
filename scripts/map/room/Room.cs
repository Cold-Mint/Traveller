using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.map.room;

/// <summary>
/// <para>Room</para>
/// <para>房间模板</para>
/// </summary>
/// <remarks>
///<para>The room template is like a jigsaw puzzle and participates in the map building process.</para>
///<para>房间模板就像一个拼图，参与到地图的构建过程中。</para>
/// </remarks>
public class Room
{
    private Node2D? _rootNode;
    private RoomSlot?[]? _roomSlots;
    private List<TileMapLayer>? _tileMapLayers;

    private Area2D? _roomArea2D;
    private Area2D? _spawnArea2D;
    private PointLight2D? _pointLight2D;
    private CollisionShape2D? _collisionShape2D;
    private bool _hasPlayer;
    /// <summary>
    /// <para>All the characters in the room</para>
    /// <para>房间内的所有角色</para>
    /// </summary>
    private readonly List<CharacterTemplate> _allCharacter = [];

    /// <summary>
    /// <para>When the player first enters the room, all <see cref="ISpawnMarker"/> nodes under this node are executed</para>
    /// <para>当玩家首次进入房间时，会执行此节点下所有<see cref="ISpawnMarker"/>节点</para>
    /// </summary>
    private Node2D? _autoSpawn;

    private WaveManager _waveManager = new WaveManager();

    /// <summary>
    /// <para>The number of times the player visits the room</para>
    /// <para>玩家访问房间的次数</para>
    /// </summary>
    private int PlayerRoomVisitCount { get; set; }

    public string? EnterRoomEventHandlerId { get; set; }

    public string? ExitRoomEventHandlerId { get; set; }

    /// <summary>
    /// <para>Get the corresponding tile layer node based on the tile name</para>
    /// <para>根据瓦片名称获取对应的瓦片图层节点</para>
    /// </summary>
    /// <param name="layerName">
    ///<para>We recommend using the constants defined in <see cref="Config.TileMapLayerName"/>.</para>
    ///<para>建议使用<see cref="Config.TileMapLayerName"/>内定义的常量。</para>
    /// </param>
    /// <returns>
    /// <para>Return a Layer node with the same name if it is found, otherwise null.</para>
    ///<para>如果找到了与其名称相同的Layer节点则返回它，否则返回null。</para>
    /// </returns>
    public TileMapLayer? GetTileMapLayer(string layerName)
    {
        return _tileMapLayers?.FirstOrDefault(tileMapLayer => tileMapLayer.Name == layerName);
    }

    /// <summary>
    /// <para>Place barriers in all slots</para>
    /// <para>在所有的槽位放置屏障</para>
    /// </summary>
    public void PlaceBarriersInAllSlots()
    {
        var barrier = GetTileMapLayer(Config.TileMapLayerName.Barrier);
        if (barrier == null)
        {
            return;
        }
        barrier.Enabled = true;
    }

    /// <summary>
    /// <para>Clear all matched barriers</para>
    /// <para>清空所有已匹配位置的屏障</para>
    /// </summary>
    public void ClearAllMatchedBarriers()
    {
        var barrier = GetTileMapLayer(Config.TileMapLayerName.Barrier);
        if (barrier == null)
        {
            return;
        }
        barrier.Enabled = false;
    }

    /// <summary>
    /// <para>Places barriers in slots that do not have a match.</para>
    /// <para>在未匹配的槽位放置屏障。</para>
    /// </summary>
    public void PlaceBarrierInUnmatchedSlots()
    {
        var ground = GetTileMapLayer(Config.TileMapLayerName.Ground);
        var barrier = GetTileMapLayer(Config.TileMapLayerName.Barrier);
        if (ground == null || barrier == null)
        {
            return;
        }

        if (RoomSlots == null || RoomSlots.Length == 0)
        {
            return;
        }

        foreach (var roomSlot in RoomSlots)
        {
            if (roomSlot == null)
            {
                continue;
            }

            //Place the corresponding coordinate tiles of the barrier layer on the ground level.
            //将屏障层的对应坐标瓦片放到地面层。
            CoordinateUtils.ForEachCell(roomSlot.StartPosition, roomSlot.EndPosition, i =>
            {
                if (roomSlot.Matched)
                {
                    return;
                }
                var cellSourceId = barrier.GetCellSourceId(i);
                if (cellSourceId == -1)
                {
                    return;
                }
                ground.SetCell(i, cellSourceId, barrier.GetCellAtlasCoords(i), barrier.GetCellAlternativeTile(i));
            });
        }
        barrier.Enabled = false;
    }


    /// <summary>
    /// <para>ShowAllCharacterTemplate</para>
    /// <para>显示所有角色模板</para>
    /// </summary>
    private void ShowAllCharacterTemplate()
    {
        LogCat.LogWithFormat("show_all_node", LogCat.LogLabel.Room, _allCharacter.Count);
        foreach (var characterTemplate in _allCharacter)
        {
            characterTemplate.Show();
        }
    }

    /// <summary>
    /// <para>HideAllCharacterTemplate</para>
    /// <para>隐藏所有角色模板</para>
    /// </summary>
    private void HideAllCharacterTemplate()
    {
        LogCat.LogWithFormat("hide_all_node", LogCat.LogLabel.Room, _allCharacter.Count);
        foreach (var characterTemplate in _allCharacter)
        {
            characterTemplate.Hide();
        }
    }


    /// <summary>
    /// <para>When a node enters the room</para>
    /// <para>当有节点进入房间时</para>
    /// </summary>
    /// <param name="node"></param>
    private void OnEnterRoom(Node node)
    {
        if (_rootNode != null)
        {
            LogCat.LogWithFormat("enter_the_room_debug", LogCat.LogLabel.Default, node.Name,
                _rootNode.Name);
        }

        if (node is Player player)
        {
            _allCharacter.Add(player);
            _hasPlayer = true;
            PlayerRoomVisitCount++;
            //The player enters the room, opening up their view.
            //玩家进入了房间，开放视野。
            if (_pointLight2D != null)
            {
                _pointLight2D.Show();
                _pointLight2D.Texture = AssetHolder.White100;
            }
            GameSceneDepend.ShowMiniMap();
            GameSceneDepend.GameGuiTemplate?.MiniMap?.ShowRoomPreview(this);
            ShowAllCharacterTemplate();
        }
        else if (node is CharacterTemplate characterTemplate)
        {
            if (_hasPlayer)
            {
                characterTemplate.Show();
            }
            else
            {
                characterTemplate.Hide();
            }

            _allCharacter.Add(characterTemplate);
        }

        if (string.IsNullOrEmpty(EnterRoomEventHandlerId))
        {
            return;
        }

        var enterRoomEventHandler = RoomEventManager.GetEnterRoomEventHandler(EnterRoomEventHandlerId);
        enterRoomEventHandler?.OnEnterRoom(PlayerRoomVisitCount, node, this);
    }


    /// <summary>
    /// <para>When a node exits the room</para>
    /// <para>当有节点退出房间时</para>
    /// </summary>
    /// <param name="node"></param>
    private void OnExitRoom(Node node)
    {
        if (_rootNode != null)
        {
            LogCat.LogWithFormat("exit_the_room_debug", LogCat.LogLabel.Default, node.Name,
                _rootNode.Name);
        }


        if (node is Player player)
        {
            //The player leaves the room, hiding the view.
            //玩家离开了房间，隐藏视野。
            _allCharacter.Remove(player);
            _hasPlayer = false;
            if (_pointLight2D != null)
            {
                _pointLight2D.Show();
                _pointLight2D.Texture = AssetHolder.White25;
            }
            HideAllCharacterTemplate();
            if (_waveManager.HasSpawnedEntity && _autoSpawn != null)
            {
                NodeUtils.ForEachNode<ISpawnMarker>(_autoSpawn, marker =>
                {
                    if (marker.CanQueueFree())
                    {
                        marker.DoQueueFree();
                    }
                    return false;
                });
            }
        }
        else if (node is CharacterTemplate characterTemplate && characterTemplate.Visible)
        {
            _allCharacter.Remove(characterTemplate);
        }

        if (string.IsNullOrEmpty(ExitRoomEventHandlerId))
        {
            return;
        }

        var exitRoomEventHandler = RoomEventManager.GetExitRoomEventHandler(ExitRoomEventHandlerId);
        exitRoomEventHandler?.OnExitRoom(node, this);
    }

    /// <summary>
    /// <para>The collision shape of the room</para>
    /// <para>房间的碰撞形状</para>
    /// </summary>
    public CollisionShape2D? RoomCollisionShape2D
    {
        get => _collisionShape2D;
        set => _collisionShape2D = value;
    }

    public Area2D? RoomArea2D
    {
        get => _roomArea2D;
        set => _roomArea2D = value;
    }

    public PackedScene? RoomScene
    {
        get => default;
        set => AnalyzeRoomData(value);
    }


    /// <summary>
    /// <para>Analyze the data of the room</para>
    /// <para>解析房间的数据</para>
    /// </summary>
    /// <param name="packedScene"></param>
    private void AnalyzeRoomData(PackedScene? packedScene)
    {
        if (packedScene == null)
        {
            return;
        }

        var rootNode = NodeUtils.InstantiatePackedScene<Node2D>(packedScene);
        if (rootNode == null)
        {
            //The room node is not of Node2D type. An exception is thrown
            //房间节点不是Node2D类型，抛出异常
            LogCat.LogError("room_root_node_must_be_node2d");
            return;
        }

        _rootNode = rootNode;
        var tileMapNode = rootNode.GetNode<Node2D>("TileMap");
        NodeUtils.ForEachNode<TileMapLayer>(tileMapNode, node =>
        {
            _tileMapLayers ??= [];
            _tileMapLayers.Add(node);
            return false;
        });
        _roomArea2D = rootNode.GetNode<Area2D>("RoomArea");
        _roomArea2D.Monitoring = true;
        _roomArea2D.SetCollisionLayerValue(Config.LayerNumber.RoomArea, true);
        _roomArea2D.SetCollisionMaskValue(Config.LayerNumber.Player, true);
        _roomArea2D.SetCollisionMaskValue(Config.LayerNumber.Mob, true);
        _roomArea2D.BodyExited += OnExitRoom;
        _roomArea2D.BodyEntered += OnEnterRoom;
        _spawnArea2D = rootNode.GetNodeOrNull<Area2D>("SpawnArea");
        if (_spawnArea2D != null)
        {
            _spawnArea2D.SetCollisionMaskValue(Config.LayerNumber.Player, true);
            _spawnArea2D.BodyEntered += OnEnterSpawnArea;
        }
        _collisionShape2D = _roomArea2D.GetChild<CollisionShape2D>(0);
        _roomSlots = GetRoomSlots(GetTileMapLayer(Config.TileMapLayerName.Ground), _roomArea2D,
            rootNode.GetNode<Node2D>("RoomSlotList"));
        _pointLight2D = rootNode.GetNodeOrNull<PointLight2D>("PointLight2D");
        if (_pointLight2D != null)
        {
            _pointLight2D.BlendMode = Light2D.BlendModeEnum.Mix;
        }
        _autoSpawn = rootNode.GetNodeOrNull<Node2D>("AutoSpawn");
    }

    private void OnEnterSpawnArea(Node2D body)
    {
        if (_waveManager.HasSpawnedEntity)
        {
            return;
        }
        if (body is not Player)
        {
            return;
        }
        _waveManager.OnWaveStart += async void () =>
        {
            try
            {
                GameSceneDepend.HideMiniMap();
                await Task.Delay(TimeSpan.FromMilliseconds(300));
                PlaceBarriersInAllSlots();
            }
            catch (Exception e)
            {
                LogCat.WhenCaughtException(e);
            }
        };
        _waveManager.OnWaveComplete += () =>
        {
            ClearAllMatchedBarriers();
            GameSceneDepend.ShowMiniMap();
        };
        _waveManager.SpawnEnemyWave(_autoSpawn);
        _waveManager.HasSpawnedEntity = true;
    }

    public Node2D? RootNode => _rootNode;

    public RoomSlot?[]? RoomSlots => _roomSlots;

    /// <summary>
    /// <para>GetRoomSlots</para>
    /// <para>在房间内获取所有插槽</para>
    /// </summary>
    /// <param name="tileMapLayer"></param>
    /// <param name="roomArea2D"></param>
    /// <param name="slotList"></param>
    /// <returns></returns>
    private RoomSlot?[]? GetRoomSlots(TileMapLayer? tileMapLayer, Area2D roomArea2D, Node2D slotList)
    {
        if (tileMapLayer == null)
        {
            return null;
        }

        var slotCount = slotList.GetChildCount();
        if (slotCount == 0)
        {
            return null;
        }

        //We calculate the midpoint of the room
        //我们计算房间的中点
        var roomAreaCollisionShape2D = roomArea2D.GetChild<CollisionShape2D>(0);
        var roomAreaRect2 = roomAreaCollisionShape2D.Shape.GetRect();
        var midpoint = roomAreaCollisionShape2D.Position + roomAreaRect2.Position + roomAreaRect2.Size / 2;
        var roomSlots = new List<RoomSlot>();
        for (var i = 0; i < slotCount; i++)
        {
            //Got the object in the room slot
            //拿到了房间槽对象
            var area2D = slotList.GetChild<Area2D>(i);
            //Prevent other areas from detecting the room slot
            //禁止其他区域检测到房间槽
            area2D.Monitorable = false;

            var collisionShape2D = area2D.GetChild<CollisionShape2D>(0);
            var rect2 = collisionShape2D.Shape.GetRect();
            //Round the size of the impactor to the tile size For example, the impactor size 44 is converted to the tile size 44/32=1.375 rounded to 1
            //将碰撞体的尺寸四舍五入到瓦片尺寸例如：碰撞体尺寸44，转为瓦片尺寸为 44/32=1.375 四舍五入为1
            var intSize = new Vector2I((int)Math.Round(rect2.Size.X / Config.CellSize),
                (int)Math.Round(rect2.Size.Y / Config.CellSize));
            if (intSize.X > 1 && intSize.Y > 1)
            {
                LogCat.LogError("width_or_height_of_room_slot_must_be_1");
                continue;
            }

            //Gets the absolute position of the slot
            //获取槽位的绝对位置
            var startPosition = area2D.Position + collisionShape2D.Position + rect2.Position;
            var endPosition = area2D.Position + collisionShape2D.Position + rect2.Position + rect2.Size;
            var midpointOfRoomSlots = area2D.Position + collisionShape2D.Position + rect2.Position + rect2.Size / 2;
            //Convert to tile map coordinates (midpoint)
            //转为瓦片地图的坐标(中点)
            var tileMapStartPosition = tileMapLayer.LocalToMap(startPosition);
            var tileMapEndPosition = tileMapLayer.LocalToMap(endPosition);
            var roomSlot = new RoomSlot
            {
                EndPosition = tileMapEndPosition,
                StartPosition = tileMapStartPosition,
                //Calculate the orientation of the slot (the midpoint of the room is the origin, the vector pointing to the midpoint of the slot)
                //计算槽位的方向(房间中点为原点，指向槽位中点的向量)
                DistanceToMidpointOfRoom = CoordinateUtils.VectorToOrientationArray(midpoint, midpointOfRoomSlots)
            };
            roomSlots.Add(roomSlot);
        }

        return roomSlots.ToArray();
    }
}