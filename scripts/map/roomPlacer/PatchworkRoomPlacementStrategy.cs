using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    /// <summary>
    /// <para>We use a temporary area to measure whether the rooms overlap</para>
    /// <para>我们使用一个临时区域进行测量房间是否重叠</para>
    /// </summary>
    private Area2D? _measuringArea2D;

    private CollisionShape2D? _measuringCollisionShape2D;

    private Area2D? _selfArea2D;

    /// <summary>
    /// <para>How many rooms overlap with the new rooms that will be placed</para>
    /// <para>有多少个房间与将要放置的新房间重叠</para>
    /// </summary>
    private int _overlapQuantity;

    public Task<bool> StartGeneration(Node mapRoot)
    {
        if (_measuringArea2D == null)
        {
            _measuringArea2D = new Area2D();
            _measuringArea2D.Monitoring = true;
            _measuringArea2D.AreaEntered += body =>
            {
                if (_selfArea2D != null && body == _selfArea2D)
                {
                    return;
                }

                //Room overlap detected
                //检测到房间重叠
                _overlapQuantity++;
            };
            _measuringArea2D.AreaExited += body =>
            {
                if (_selfArea2D != null && body == _selfArea2D)
                {
                    return;
                }

                //Rooms no longer overlap
                //房间不再重叠
                _overlapQuantity--;
            };
            mapRoot.AddChild(_measuringArea2D);
        }

        if (_measuringCollisionShape2D == null)
        {
            _measuringCollisionShape2D = new CollisionShape2D();
            _measuringArea2D.AddChild(_measuringCollisionShape2D);
        }

        return Task.FromResult(true);
    }

    public Task GeneratedComplete(Node mapRoot)
    {
        if (_measuringCollisionShape2D != null)
        {
            _measuringCollisionShape2D?.QueueFree();
            _measuringArea2D?.RemoveChild(_measuringCollisionShape2D);
            _measuringCollisionShape2D = null;
        }

        if (_measuringArea2D != null)
        {
            _measuringArea2D?.QueueFree();
            mapRoot.RemoveChild(_measuringArea2D);
            _measuringArea2D = null;
        }

        return Task.CompletedTask;
    }


    public Task<bool> PlaceRoom(Node mapRoot, RoomPlacementData roomPlacementData)
    {
        if (roomPlacementData.NewRoom == null || roomPlacementData.Position == null)
        {
            return Task.FromResult(false);
        }

        if (roomPlacementData.NewRoom.RootNode == null)
        {
            return Task.FromResult(false);
        }

        var newRootRootNode = roomPlacementData.NewRoom.RootNode;
        mapRoot.AddChild(newRootRootNode);
        newRootRootNode.Position = roomPlacementData.Position.Value;
        //Place navigation Link
        //放置导航Link
        Vector2? navigationLink2DStartPosition = null;
        if (roomPlacementData is { ParentRoom: not null, ParentRoomSlot: not null })
        {
            var parentRoomTileMap = roomPlacementData.ParentRoom.GetTileMapLayer(Config.TileMapLayerName.Ground);
            var parentRoomRootNode = roomPlacementData.ParentRoom.RootNode;
            if (parentRoomTileMap != null && parentRoomRootNode != null)
            {
                navigationLink2DStartPosition = parentRoomRootNode.Position +
                                                parentRoomTileMap.MapToLocal(roomPlacementData.ParentRoomSlot
                                                    .EndPosition);
            }
        }

        Vector2? navigationLink2DEndPosition = null;
        if (roomPlacementData.NewRoomSlot != null)
        {
            var newRoomTileMap = roomPlacementData.NewRoom.GetTileMapLayer(Config.TileMapLayerName.Ground);
            if (newRoomTileMap != null)
            {
                navigationLink2DEndPosition = newRootRootNode.Position +
                                              newRoomTileMap.MapToLocal(roomPlacementData.NewRoomSlot.EndPosition);
            }
        }

        if (navigationLink2DStartPosition != null && navigationLink2DEndPosition != null)
        {
            var navigationLink2D = new NavigationLink2D();
            navigationLink2D.StartPosition = navigationLink2DStartPosition.Value;
            navigationLink2D.EndPosition = navigationLink2DEndPosition.Value;
            mapRoot.AddChild(navigationLink2D);
        }

        return Task.FromResult(true);
    }

    public async Task<RoomPlacementData?> CalculateNewRoomPlacementData(RandomNumberGenerator randomNumberGenerator,
        Room? parentRoomNode,
        RoomNodeData newRoomNodeData)
    {
        if (newRoomNodeData.RoomTemplateSet == null || newRoomNodeData.RoomTemplateSet.Length == 0)
        {
            return null;
        }

        if (parentRoomNode == null)
        {
            return null;
        }

        var roomResArray = RoomFactory.RoomTemplateSetToRoomRes(newRoomNodeData.RoomTemplateSet);
        if (roomResArray.Length == 0)
        {
            return null;
        }

        var roomSlots = parentRoomNode.RoomSlots;
        if (roomSlots == null || roomSlots.Length == 0)
        {
            return null;
        }

        //Saves all data in the room template that matches the parent room.
        //保存房间模板内所有与父房间匹配的数据。
        var usableRoomPlacementData = new List<RoomPlacementData>();
        foreach (var roomRes in roomResArray)
        {
            var newRoom = RoomFactory.CreateRoom(roomRes, newRoomNodeData.EnterRoomEventHandlerId,
                newRoomNodeData.ExitRoomEventHandlerId);
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

            var position = await CalculatedPosition(parentRoomNode, newRoom, mainRoomSlot, newRoomSlot, false);
            if (position == null) continue;
            var roomPlacementData = new RoomPlacementData
            {
                NewRoom = newRoom,
                ParentRoom = parentRoomNode,
                Position = position,
                ParentRoomSlot = mainRoomSlot,
                NewRoomSlot = newRoomSlot
            };
            usableRoomPlacementData.Add(roomPlacementData);
        }

        if (usableRoomPlacementData.Count == 0)
        {
            return null;
        }
        else
        {
            var index = randomNumberGenerator.Randi() % usableRoomPlacementData.Count;
            var roomPlacementData = usableRoomPlacementData[(int)index];
            return roomPlacementData;
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
            NewRoom = RoomFactory.CreateRoom(roomResArray[index], startRoomNodeData.EnterRoomEventHandlerId,
                startRoomNodeData.ExitRoomEventHandlerId),
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
                //If it's already a match, it's no longer a match
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
                    //If it's already a match, it's no longer a match
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

                outMainRoomSlot = mainRoomSlot;
                outNewRoomSlot = newRoomSlot;
                return Task.FromResult(true);
            }
        }

        outNewRoomSlot = null;
        outMainRoomSlot = null;
        return Task.FromResult(false);
    }

    /// <summary>
    /// <para>Calculate room position</para>
    /// <para>计算房间位置</para>
    /// </summary>
    /// <param name="mainRoom">
    ///<para>Main room</para>
    ///<para>主房间</para>
    /// </param>
    /// <param name="newRoom">
    ///<para>New room</para>
    ///<para>新房间</para>
    /// </param>
    /// <param name="mainRoomSlot">
    ///<para>Main room slot</para>
    ///<para>主房间插槽</para>
    /// </param>
    /// <param name="newRoomSlot">
    ///<para>New room slot</para>
    ///<para>新房间插槽</para>
    /// </param>
    /// <param name="roomSlotOverlap">
    ///<para>Whether room slots allow overlays</para>
    ///<para>房间插槽是否允许覆盖</para>
    /// </param>
    /// <returns></returns>
    private async Task<Vector2?> CalculatedPosition(Room mainRoom, Room newRoom, RoomSlot? mainRoomSlot,
        RoomSlot? newRoomSlot, bool roomSlotOverlap)
    {
        var mainRoomTileMapLayer = mainRoom.GetTileMapLayer(Config.TileMapLayerName.Ground);
        var newRoomTileMapLayer = newRoom.GetTileMapLayer(Config.TileMapLayerName.Ground);
        if (mainRoom.RootNode == null || newRoom.RootNode == null ||
            mainRoomTileMapLayer == null ||
            newRoomTileMapLayer == null || mainRoomSlot == null ||
            newRoomSlot == null)
        {
            return null;
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
            return null;
        }

        var mainRoomSlotPosition = mainRoomTileMapLayer
            .MapToLocal(mainRoomSlot.StartPosition);
        var newRoomSlotPosition = newRoomTileMapLayer
            .MapToLocal(newRoomSlot.StartPosition);
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

        //Do calculations overlap with other rooms?
        //计算结果是否与其他房间重叠？
        if (newRoom.RoomCollisionShape2D != null && _measuringArea2D != null && _measuringCollisionShape2D != null)
        {
            //Ignore yourself when detecting room overlap
            //检测房间重叠时应忽略自身
            _selfArea2D = newRoom.RoomArea2D;
            _measuringArea2D.Position = result;
            _measuringCollisionShape2D.Shape = newRoom.RoomCollisionShape2D.Shape;
            //Calculate the offset of the shape.
            //计算形状的偏移量。
            _measuringCollisionShape2D.Position = newRoom.RoomCollisionShape2D.Shape.GetRect().Size / 2;
            await Task.Delay(TimeSpan.FromMilliseconds(50));
            if (_overlapQuantity > 0)
            {
                return null;
            }
        }


        return result;
    }
}