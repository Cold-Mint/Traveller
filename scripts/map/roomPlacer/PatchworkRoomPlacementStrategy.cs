using System.Threading.Tasks;
using ColdMint.scripts.levelGraphEditor;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.map.interfaces;
using ColdMint.scripts.map.room;
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
    private readonly Vector2 _halfCell = new Vector2(Config.CellSize / 2f, Config.CellSize / 2f);

    public Task<bool> PlaceRoom(Node mapRoot, RoomPlacementData roomPlacementData)
    {
        if (roomPlacementData.Room == null || roomPlacementData.Position == null)
        {
            return Task.FromResult(false);
        }

        if (roomPlacementData.Room.RootNode == null)
        {
            return Task.FromResult(false);
        }

        var rootNode = roomPlacementData.Room.RootNode;
        rootNode.Reparent(mapRoot);
        rootNode.Position = roomPlacementData.Position.Value;
        return Task.FromResult(true);
    }

    public Task<RoomPlacementData?> CalculateNewRoomPlacementData(Room? parentRoomNode, RoomNodeData newRoomNodeData)
    {
        if (newRoomNodeData.RoomTemplateSet == null || newRoomNodeData.RoomTemplateSet.Length == 0)
        {
            return Task.FromResult<RoomPlacementData?>(null);
        }

        var roomResArray = RoomFactory.RoomTemplateSetToRoomRes(newRoomNodeData.RoomTemplateSet);
        if (parentRoomNode == null)
        {
            //No parent node is set, which we think is the starting room.
            //没有设置父节点，我们认为是起点房间。
            //TODO:在这里兼容世界种子。
            var roomPlacementData = new RoomPlacementData
            {
                Room = RoomFactory.CreateRoom(roomResArray[0]),
                Position = Vector2.Zero
            };
            return Task.FromResult<RoomPlacementData?>(roomPlacementData);
        }
        else
        {
            //TODO:在这里实现房间的放置策略。
            return Task.FromResult<RoomPlacementData?>(null);
        }
    }

    private Task<Vector2?> CalculatedPosition(Room mainRoom, Room newRoom, RoomSlot? mainRoomSlot,
        RoomSlot? newRoomSlot,bool roomSlotOverlap)
    {
        if (mainRoom.RootNode == null || mainRoom.TileMap == null || newRoom.TileMap == null || mainRoomSlot == null ||
            newRoomSlot == null)
        {
            return Task.FromResult<Vector2?>(null);
        }

        //计算主插槽中点在世界中的位置。
        //mainRoom.RootNode.Position意为房间所在的世界位置
        //mainRoom.TileMap.MapToLocal(mainRoomSlot.StartPosition)意为主插槽在房间中的位置
        var result = mainRoom.RootNode.Position + mainRoom.TileMap.MapToLocal(mainRoomSlot.StartPosition);
        if (roomSlotOverlap)
        {
            //执行减法，从槽中点偏移到左上角
            result -= _halfCell;
        }
        else
        {
            //执行减法，从槽中点偏移到右下角
            result += _halfCell;
        }
        //我们不能将新房间的原点设置在主房间槽的左上角或右下角，这会导致插槽不对应。

        //竖直槽，我们需要在同一水平上。
        if (mainRoomSlot.IsHorizontal)
        {
            result += newRoom.TileMap.MapToLocal(new Vector2I(newRoomSlot.EndPosition.X, 0)) - _halfCell;
        }
        else
        {
            result -= newRoom.TileMap.MapToLocal(new Vector2I(0, newRoomSlot.EndPosition.Y)) - _halfCell;
        }

        return Task.FromResult<Vector2?>(result);
    }
}