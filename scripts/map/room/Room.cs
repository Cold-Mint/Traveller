using System;
using System.Collections.Generic;
using ColdMint.scripts.debug;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.map.interfaces;
using ColdMint.scripts.utils;
using Godot;
using Godot.Collections;

namespace ColdMint.scripts.map.room;

/// <summary>
/// <para>Room</para>
/// <para>房间模板</para>
/// </summary>
/// <remarks>
///<para>The room template is like a jigsaw puzzle and participates in the map building process.</para>
///<para>房间模板就像一个拼图，参与到地图的构建过程中。</para>
/// </remarks>
public class Room : IRoom
{
    private Node2D _rootNode;
    private RoomSlot[] _roomSlots;
    private TileMap _tileMap;

    public PackedScene RoomScene
    {
        get => default;
        set => AnalyzeRoomData(value);
    }

    public TileMap TileMap
    {
        get => _tileMap;
        set => _tileMap = value;
    }

    /// <summary>
    /// <para>Analyze the data of the room</para>
    /// <para>解析房间的数据</para>
    /// </summary>
    /// <param name="packedScene"></param>
    private void AnalyzeRoomData(PackedScene packedScene)
    {
        var node = packedScene.Instantiate();
        if (node is not Node2D node2D)
        {
            //房间节点不是Node2D类型，抛出异常
            LogCat.LogError("room_root_node_must_be_node2d");
            return;
        }

        _rootNode = node2D;
        _tileMap = node2D.GetNode<TileMap>("TileMap");
        _roomSlots = GetRoomSlots(_tileMap, node2D.GetNode<Area2D>("RoomArea"),
            node2D.GetNode<Node2D>("RoomSlotList"));
    }

    public Node2D RootNode => _rootNode;

    public RoomSlot[] RoomSlots => _roomSlots;

    /// <summary>
    /// <para>GetRoomSlots</para>
    /// <para>在房间内获取所有插槽</para>
    /// </summary>
    /// <param name="tileMap"></param>
    /// <param name="slotList"></param>
    /// <returns></returns>
    private RoomSlot[] GetRoomSlots(TileMap tileMap, Area2D roomArea2D, Node2D slotList)
    {
        var slotCount = slotList.GetChildCount();
        if (slotCount == 0)
        {
            return null;
        }

        //region 我们计算房间的中点
        var roomAreaCollisionShape2D = roomArea2D.GetChild<CollisionShape2D>(0);
        var roomAreaRect2 = roomAreaCollisionShape2D.Shape.GetRect();
        var midpoint = roomAreaCollisionShape2D.Position + roomAreaRect2.Position + roomAreaRect2.Size / 2;
        //endregion
        var roomSlots = new List<RoomSlot>();
        for (int i = 0; i < slotCount; i++)
        {
            //拿到了房间卡槽对象
            var area2D = slotList.GetChild<Area2D>(i);
            var collisionShape2D = area2D.GetChild<CollisionShape2D>(0);
            var rect2 = collisionShape2D.Shape.GetRect();
            //将碰撞体的尺寸四舍五入到瓦片尺寸例如：碰撞体尺寸44，转为瓦片尺寸为 44/32=1.375 四舍五入为1
            var intSize = new Vector2I((int)Math.Round(rect2.Size.X / Config.CellSize),
                (int)Math.Round(rect2.Size.Y / Config.CellSize));
            if (intSize.X > 1 && intSize.Y > 1)
            {
                LogCat.LogError("width_or_height_of_room_slot_must_be_1");
                continue;
            }

            //获取槽位的绝对位置
            var startPosition = area2D.Position + collisionShape2D.Position + rect2.Position;
            var endPosition = area2D.Position + collisionShape2D.Position + rect2.Position + rect2.Size;
            var midpointOfRoomSlots = area2D.Position + collisionShape2D.Position + rect2.Position + rect2.Size / 2;
            //转为瓦片地图的坐标(中点)
            var tileMapStartPosition = tileMap.LocalToMap(startPosition);
            var tileMapEndPosition = tileMap.LocalToMap(endPosition);
            var roomSlot = new RoomSlot();
            roomSlot.EndPosition = tileMapEndPosition;
            roomSlot.StartPosition = tileMapStartPosition;
            //计算槽位的方向(房间中点为原点，指向槽位中点的向量)
            roomSlot.DistanceToMidpointOfRoom = CoordinateUtils.VectorToOrientationArray(midpoint, midpointOfRoomSlots);
            roomSlots.Add(roomSlot);
        }

        return roomSlots.ToArray();
    }
}