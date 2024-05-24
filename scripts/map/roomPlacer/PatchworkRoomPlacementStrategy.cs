using System.Collections.Generic;
using System.Threading.Tasks;
using ColdMint.scripts.debug;
using ColdMint.scripts.levelGraphEditor;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.map.interfaces;
using ColdMint.scripts.map.room;
using ColdMint.scripts.utils;
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
        mapRoot.AddChild(rootNode);
        rootNode.Position = roomPlacementData.Position.Value;
        return Task.FromResult(true);
    }

    public Task<RoomPlacementData?> CalculateNewRoomPlacementData(RandomNumberGenerator randomNumberGenerator,
        Room? parentRoomNode,
        RoomNodeData newRoomNodeData)
    {
        if (newRoomNodeData.RoomTemplateSet == null || newRoomNodeData.RoomTemplateSet.Length == 0)
        {
            return Task.FromResult<RoomPlacementData?>(null);
        }

        if (parentRoomNode == null)
        {
            return Task.FromResult<RoomPlacementData?>(null);
        }

        var roomResArray = RoomFactory.RoomTemplateSetToRoomRes(newRoomNodeData.RoomTemplateSet);
        if (roomResArray.Length == 0)
        {
            return Task.FromResult<RoomPlacementData?>(null);
        }

        var roomSlots = parentRoomNode.RoomSlots;
        if (roomSlots == null || roomSlots.Length == 0)
        {
            return Task.FromResult<RoomPlacementData?>(null);
        }

        //Saves all data in the room template that matches the parent room.
        //保存房间模板内所有与父房间匹配的数据。
        var useableRoomPlacementData = new List<RoomPlacementData>();
        foreach (var roomRes in roomResArray)
        {
            var newRoom = RoomFactory.CreateRoom(roomRes);
            if (newRoom == null)
            {
                continue;
            }

            //Create a room, try to use the room slot to match the existing room slot.
            //创建了一个房间，尝试使用房间的槽与现有的房间槽匹配。
            if (!IsMatch(parentRoomNode, newRoom, out var mainRoomSlot, out var newRoomSlot).Result)
            {
                continue;
            }

            if (mainRoomSlot == null || newRoomSlot == null)
            {
                continue;
            }

            var position = CalculatedPosition(parentRoomNode, newRoom, mainRoomSlot, newRoomSlot, false)
                .Result;
            if (position == null) continue;
            var roomPlacementData = new RoomPlacementData
            {
                Room = newRoom,
                Position = position
            };
            useableRoomPlacementData.Add(roomPlacementData);
        }

        if (useableRoomPlacementData.Count == 0)
        {
            return Task.FromResult<RoomPlacementData?>(null);
        }
        else
        {
            var index = randomNumberGenerator.Randi() % useableRoomPlacementData.Count;
            return Task.FromResult<RoomPlacementData?>(useableRoomPlacementData[(int)index]);
        }
    }

    public Task<RoomPlacementData?> CalculatePlacementDataForStartingRoom(RandomNumberGenerator randomNumberGenerator,
        RoomNodeData startRoomNodeData)
    {
        if (startRoomNodeData.RoomTemplateSet == null || startRoomNodeData.RoomTemplateSet.Length == 0)
        {
            return Task.FromResult<RoomPlacementData?>(null);
        }

        var roomResArray = RoomFactory.RoomTemplateSetToRoomRes(startRoomNodeData.RoomTemplateSet);
        if (roomResArray.Length == 0)
        {
            return Task.FromResult<RoomPlacementData?>(null);
        }

        var index = randomNumberGenerator.Randi() % roomResArray.Length;
        var roomPlacementData = new RoomPlacementData
        {
            Room = RoomFactory.CreateRoom(roomResArray[index]),
            Position = Vector2.Zero
        };
        return Task.FromResult<RoomPlacementData?>(roomPlacementData);
    }


    /// <summary>
    /// <para>if it matches</para>
    /// <para>是否匹配</para>
    /// </summary>
    /// <param name="mainRoom"></param>
    /// <param name="newRoom"></param>
    /// <param name="outMainRoomSlot"></param>
    /// <param name="outNewRoomSlot"></param>
    /// <returns></returns>
    public Task<bool> IsMatch(Room? mainRoom, Room newRoom, out RoomSlot? outMainRoomSlot, out RoomSlot? outNewRoomSlot)
    {
        if (mainRoom == null)
        {
            outNewRoomSlot = null;
            outMainRoomSlot = null;
            return Task.FromResult(false);
        }

        var roomSlots = mainRoom.RoomSlots;
        if (roomSlots == null)
        {
            outNewRoomSlot = null;
            outMainRoomSlot = null;
            return Task.FromResult(false);
        }

        var newRoomSlots = newRoom.RoomSlots;
        if (newRoomSlots == null)
        {
            outNewRoomSlot = null;
            outMainRoomSlot = null;
            return Task.FromResult(false);
        }

        foreach (var mainRoomSlot in roomSlots)
        {
            if (mainRoomSlot == null || mainRoomSlot.Matched)
            {
                //如果已经匹配过了，就不再匹配
                continue;
            }

            foreach (var newRoomSlot in newRoomSlots)
            {
                if (newRoomSlot == null)
                {
                    continue;
                }

                if (newRoomSlot.Matched)
                {
                    //如果已经匹配过了，就不再匹配
                    continue;
                }

                if (mainRoomSlot.IsHorizontal != newRoomSlot.IsHorizontal)
                {
                    continue;
                }

                if (mainRoomSlot.Length != newRoomSlot.Length)
                {
                    continue;
                }

                var distanceToMidpointOfRoom = mainRoomSlot.DistanceToMidpointOfRoom;
                var newDistanceToMidpointOfRoom = newRoomSlot.DistanceToMidpointOfRoom;
                if (distanceToMidpointOfRoom == null || newDistanceToMidpointOfRoom == null)
                {
                    continue;
                }

                if (distanceToMidpointOfRoom[0] == newDistanceToMidpointOfRoom[0] &&
                    distanceToMidpointOfRoom[1] == newDistanceToMidpointOfRoom[1])
                {
                    continue;
                }
                
                mainRoomSlot.Matched = true;
                newRoomSlot.Matched = true;
                outMainRoomSlot = mainRoomSlot;
                outNewRoomSlot = newRoomSlot;
                return Task.FromResult(true);
            }
        }

        outNewRoomSlot = null;
        outMainRoomSlot = null;
        return Task.FromResult(false);
    }

    private Task<Vector2?> CalculatedPosition(Room mainRoom, Room newRoom, RoomSlot? mainRoomSlot,
        RoomSlot? newRoomSlot, bool roomSlotOverlap)
    {
        if (mainRoom.RootNode == null || newRoom.RootNode == null || newRoom.TileMap == null ||
            mainRoom.TileMap == null ||
            newRoom.TileMap == null || mainRoomSlot == null ||
            newRoomSlot == null)
        {
            return Task.FromResult<Vector2?>(null);
        }

        //Main room slot location description
        //主房间槽位置描述
        var mainOrientationDescribe = mainRoomSlot.DistanceToMidpointOfRoom;
        //New room slot location description
        //新房间槽位置描述
        var newOrientationDescribe = newRoomSlot.DistanceToMidpointOfRoom;
        if (mainOrientationDescribe == null || newOrientationDescribe == null)
        {
            //If the room slot is described as null, null is returned
            //若房间槽描述为null，那么返回null
            return Task.FromResult<Vector2?>(null);
        }

        var mainRoomSlotPosition = mainRoom.TileMap.MapToLocal(mainRoomSlot.StartPosition);
        var newRoomSlotPosition = newRoom.TileMap.MapToLocal(newRoomSlot.StartPosition);
        //Get the vector from the new room slot to the main room slot
        //得到从新房间槽位到主房间槽位的向量
        var newToMain = mainRoomSlotPosition - newRoomSlotPosition;
        var result = newToMain + mainRoom.RootNode.Position;
        if (!roomSlotOverlap)
        {
            //如果不允许房间槽位重叠
            //If room slot overlap is not allowed
            if (mainRoomSlot.IsHorizontal)
            {
                //Horizontal slot, offset in the Y direction.
                //水平方向槽，向Y方向偏移。
                if (newOrientationDescribe[1] == CoordinateUtils.OrientationDescribe.Up)
                {
                    result.Y += Config.CellSize;
                }
                else
                {
                    result.Y -= Config.CellSize;
                }
            }
            else
            {
                //Vertical slot, offset in the X direction.
                //垂直方向槽向X方向偏移。
                if (newOrientationDescribe[0] == CoordinateUtils.OrientationDescribe.Right)
                {
                    result.X -= Config.CellSize;
                }
                else
                {
                    result.X += Config.CellSize;
                }
            }
        }
        return Task.FromResult<Vector2?>(result);
    }
}