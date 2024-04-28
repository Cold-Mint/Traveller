using System.Collections.Generic;
using ColdMint.scripts.debug;
using ColdMint.scripts.map.interfaces;
using ColdMint.scripts.map.room;
using Godot;

namespace ColdMint.scripts.map.RoomProvider;

public class RoomProvider : IRoomProvider
{
    private List<RoomTemplate> _roomTemplates;

    public RoomProvider()
    {
        _roomTemplates = new List<RoomTemplate>();
    }

    /// <summary>
    /// <para>AddRoom</para>
    /// <para>添加房间</para>
    /// </summary>
    /// <remarks>
    ///<para>If the initial room is not set, the first room added will be automatically set as the initial room</para>
    ///<para>若未设置初始房间，那么第一次添加的房间将被自动设置为初始房间</para>
    /// </remarks>
    /// <param name="resPath"></param>
    public void AddRoom(RoomTemplate roomTemplate)
    {
        if (InitialRoom == null)
        {
            InitialRoom = roomTemplate;
            return;
        }

        _roomTemplates.Add(roomTemplate);
    }


    public IRoomTemplate InitialRoom { get; set; }

    public IRoomTemplate GetRoomRes(int index, IMapGeneratorConfig config)
    {
        var indexInList = config.RandomNumberGenerator.RandiRange(0, _roomTemplates.Count - 1);
        LogCat.Log("种子" + config.Seed + "获取" + index + "返回" + indexInList);
        IRoomTemplate result = _roomTemplates[indexInList];
        //添加一次使用次数，当模板不能再次使用时，从列表内移除。
        result.AddUsedNumber();
        if (!result.CanUse)
        {
            _roomTemplates.RemoveAt(indexInList);
        }

        return result;
    }
}