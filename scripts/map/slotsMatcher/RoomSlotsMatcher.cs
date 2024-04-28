using System.Threading.Tasks;
using ColdMint.scripts.debug;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.map.interfaces;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.map.slotsMatcher;

public class RoomSlotsMatcher : IRoomSlotsMatcher
{
    private RoomSlot _lastMatchedMainSlot;
    private RoomSlot _lastMatchedMinorSlot;

    public async Task<bool> IsMatch(IRoom mainRoom, IRoom newRoom)
    {
        foreach (var mainRoomSlot in mainRoom.RoomSlots)
        {
            if (mainRoomSlot.Matched)
            {
                //如果已经匹配过了，就不再匹配
                continue;
            }

            foreach (var newRoomSlot in newRoom.RoomSlots)
            {
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
                LogCat.Log("尝试匹配" + distanceToMidpointOfRoom[0] + " " + distanceToMidpointOfRoom[1] + " 到 " +
                           newDistanceToMidpointOfRoom[0] + " " + newDistanceToMidpointOfRoom[1]);
                if (distanceToMidpointOfRoom[0] == newDistanceToMidpointOfRoom[0] &&
                    distanceToMidpointOfRoom[1] == newDistanceToMidpointOfRoom[1])
                {
                    continue;
                }

                mainRoomSlot.Matched = true;
                newRoomSlot.Matched = true;
                _lastMatchedMainSlot = mainRoomSlot;
                _lastMatchedMinorSlot = newRoomSlot;
                return true;
            }
        }

        return false;
    }

    public RoomSlot LastMatchedMainSlot => _lastMatchedMainSlot;
    public RoomSlot LastMatchedMinorSlot => _lastMatchedMinorSlot;
}