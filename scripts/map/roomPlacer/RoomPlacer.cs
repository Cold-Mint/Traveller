using System.Threading.Tasks;
using ColdMint.scripts.debug;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.map.interfaces;
using Godot;
using static ColdMint.scripts.Config;

namespace ColdMint.scripts.map.RoomPlacer;

public class RoomPlacer : RoomPlacerTemplate
{
    public IMapGeneratorConfig MapGeneratorConfig { get; set; }
    private Vector2 _halfCell = new Vector2(CellSize / 2, CellSize / 2);

    public async override Task<bool> PlaceRoom(Vector2 position, IRoom room)
    {
        var node = room.RootNode;
        MapGeneratorConfig.MapRoot.AddChild(node);
        if (node is Node2D node2D)
        {
            node2D.Position = position;
            return true;
        }

        return false;
    }

    public async override Task<Vector2> CalculatedPosition(IRoom mainRoom, IRoom newRoom, RoomSlot mainRoomSlot,
        RoomSlot newRoomSlot,
        RoomPlacerConfig roomPlacerConfig)
    {
        //计算主插槽中点在世界中的位置。
        //mainRoom.RootNode.Position意为房间所在的世界位置
        //mainRoom.TileMap.MapToLocal(mainRoomSlot.StartPosition)意为主插槽在房间中的位置
        var result = mainRoom.RootNode.Position + mainRoom.TileMap.MapToLocal(mainRoomSlot.StartPosition);
        if (roomPlacerConfig.RoomSlotOverlap)
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

        return result;
    }
}