using System;
using System.Collections.Generic;
using System.Linq;
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

    private Area2D? _area2D;
    private PointLight2D? _pointLight2D;
    private CollisionShape2D? _collisionShape2D;
    private bool _hasPlayer;
    private readonly List<CharacterTemplate> _characterTemplateList = [];

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
    /// <para>ShowAllCharacterTemplate</para>
    /// <para>显示所有角色模板</para>
    /// </summary>
    private void ShowAllCharacterTemplate()
    {
        LogCat.LogWithFormat("show_all_node", LogCat.LogLabel.Room, LogCat.UploadFormat,
            _characterTemplateList.Count);
        foreach (var characterTemplate in _characterTemplateList)
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
        LogCat.LogWithFormat("hide_all_node", LogCat.LogLabel.Room, LogCat.UploadFormat,
            _characterTemplateList.Count);
        foreach (var characterTemplate in _characterTemplateList)
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
            LogCat.LogWithFormat("enter_the_room_debug", LogCat.LogLabel.Default, LogCat.UploadFormat, node.Name,
                _rootNode.Name);
        }

        if (node is Player player)
        {
            _characterTemplateList.Add(player);
            _hasPlayer = true;
            //The player enters the room, opening up their view.
            //玩家进入了房间，开放视野。
            if (_pointLight2D != null)
            {
                _pointLight2D.Show();
                _pointLight2D.Texture = AssetHolder.White100;
            }

            ShowAllCharacterTemplate();
        }else if (node is CharacterTemplate characterTemplate)
        {
            if (_hasPlayer)
            {
                characterTemplate.Show();
            }
            else
            {
                characterTemplate.Hide();
            }

            _characterTemplateList.Add(characterTemplate);
        }

        if (string.IsNullOrEmpty(EnterRoomEventHandlerId))
        {
            return;
        }

        var enterRoomEventHandler = RoomEventManager.GetEnterRoomEventHandler(EnterRoomEventHandlerId);
        enterRoomEventHandler?.OnEnterRoom(node, this);
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
            LogCat.LogWithFormat("exit_the_room_debug", LogCat.LogLabel.Default, LogCat.UploadFormat, node.Name,
                _rootNode.Name);
        }


        if (node is Player player)
        {
            //The player leaves the room, hiding the view.
            //玩家离开了房间，隐藏视野。
            _characterTemplateList.Remove(player);
            _hasPlayer = false;
            if (_pointLight2D != null)
            {
                _pointLight2D.Show();
                _pointLight2D.Texture = AssetHolder.White25;
            }

            HideAllCharacterTemplate();
        }
        else if (node is CharacterTemplate characterTemplate && characterTemplate.Visible)
        {
            _characterTemplateList.Remove(characterTemplate);
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

    public Area2D? Area2D
    {
        get => _area2D;
        set => _area2D = value;
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

        var node2D = NodeUtils.InstantiatePackedScene<Node2D>(packedScene);
        if (node2D == null)
        {
            //The room node is not of Node2D type. An exception is thrown
            //房间节点不是Node2D类型，抛出异常
            LogCat.LogError("room_root_node_must_be_node2d");
            return;
        }

        _rootNode = node2D;
        var tileMapNode = node2D.GetNode<Node2D>("TileMap");
        NodeUtils.ForEachNode<TileMapLayer>(tileMapNode, node =>
        {
            _tileMapLayers ??= [];
            _tileMapLayers.Add(node);
            return false;
        });
        _area2D = node2D.GetNode<Area2D>("RoomArea");
        _area2D.Monitoring = true;
        _area2D.SetCollisionLayerValue(Config.LayerNumber.RoomArea, true);
        //Sets the collision layer that can be detected in the current room area.
        //设置当前房间区域可检测到的碰撞层。
        _area2D.SetCollisionMaskValue(Config.LayerNumber.Player, true);
        _area2D.SetCollisionMaskValue(Config.LayerNumber.Mob, true);
        _area2D.BodyExited += OnExitRoom;
        _area2D.BodyEntered += OnEnterRoom;
        _collisionShape2D = _area2D.GetChild<CollisionShape2D>(0);
        _roomSlots = GetRoomSlots(GetTileMapLayer(Config.TileMapLayerName.Ground), _area2D,
            node2D.GetNode<Node2D>("RoomSlotList"));
        _pointLight2D = node2D.GetNodeOrNull<PointLight2D>("PointLight2D");
        if (_pointLight2D != null)
        {
            _pointLight2D.BlendMode = Light2D.BlendModeEnum.Mix;
        }
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