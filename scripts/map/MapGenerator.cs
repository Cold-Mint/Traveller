using System;
using System.Threading.Tasks;
using ColdMint.scripts.debug;
using ColdMint.scripts.map.interfaces;
using ColdMint.scripts.map.room;
using ColdMint.scripts.map.RoomPlacer;
using Godot;

namespace ColdMint.scripts.map;

public class MapGenerator : IMapGenerator
{
    public int TimeOutPeriod { get; set; }
    public IRoomSlotsMatcher? RoomSlotsMatcher { get; set; }
    public IRoomHolder? RoomHolder { get; set; }

    public IRoomPlacer? RoomPlacer { get; set; }

    public IRoomProvider? RoomProvider { get; set; }

    public async Task Generate(IMapGeneratorConfig mapGeneratorConfig)
    {
        if (RoomPlacer == null || RoomHolder == null || RoomProvider == null || RoomSlotsMatcher == null ||
            RoomProvider.InitialRoom == null)
        {
            PrintMissingParametersError();
            return;
        }

        try
        {
            var roomPlacerConfig = new RoomPlacerConfig();
            //获取原点
            var origin = Vector2.Zero;
            //在提供者哪里获取房间，并放置他（首次拿初始房间）
            var originRoom =
                RoomFactory.CreateRoom(RoomProvider.InitialRoom.RoomResPath);
            await PlaceRoom(origin, originRoom);
            var endTime = DateTime.Now + TimeSpan.FromSeconds(TimeOutPeriod);
            while (RoomHolder.PlacedRoomNumber < mapGeneratorConfig.RoomCount)
            {
                if (DateTime.Now > endTime)
                {
                    LogCat.LogError("connected_room_timeout");
                    break;
                }

                //我们会一直尝试放置房间，直到达到指定的数量
                var roomRes = RoomProvider.GetRoomRes(RoomHolder.PlacedRoomNumber, mapGeneratorConfig);
                if (roomRes == null)
                {
                    continue;
                }

                var newRoom =
                    RoomFactory.CreateRoom(roomRes
                        .RoomResPath);
                if (await RoomSlotsMatcher.IsMatch(RoomHolder.LastRoom, newRoom))
                {
                    // LogCat.Log("匹配成功" + RoomSlotsMatcher.LastMatchedMainSlot.DistanceToMidpointOfRoom[0] + " " +
                    //            RoomSlotsMatcher.LastMatchedMainSlot.DistanceToMidpointOfRoom[1] + "到" +
                    //            RoomSlotsMatcher.LastMatchedMinorSlot.DistanceToMidpointOfRoom[0] + " " +
                    //            RoomSlotsMatcher.LastMatchedMinorSlot.DistanceToMidpointOfRoom[1]);
                    await PlaceRoom(
                        await RoomPlacer.CalculatedPosition(originRoom, newRoom, RoomSlotsMatcher.LastMatchedMainSlot,
                            RoomSlotsMatcher.LastMatchedMinorSlot, roomPlacerConfig), newRoom);
                    originRoom = newRoom;
                }
            }
        }
        catch (Exception e)
        {
            LogCat.WhenCaughtException(e);
        }
    }

    /// <summary>
    /// <para>PrintMissingParametersError</para>
    /// <para>打印缺少参数错误</para>
    /// </summary>
    private void PrintMissingParametersError()
    {
        LogCat.LogError("missing_parameters");
    }

    /// <summary>
    /// <para>PlaceRoom</para>
    /// <para>放置房间</para>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="room"></param>
    /// <returns></returns>
    private async Task<bool> PlaceRoom(Vector2 position, IRoom room)
    {
        if (RoomPlacer == null || RoomHolder == null)
        {
            return false;
        }

        if (await RoomPlacer.PlaceRoom(position, room))
        {
            RoomHolder.AddRoom(room);
            // LogCat.Log("我要放置房间，但是成功");
            return true;
        }

        // LogCat.Log("我要放置房间，但是失败了");
        return false;
    }
}