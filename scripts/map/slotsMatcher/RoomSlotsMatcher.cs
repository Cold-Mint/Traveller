using System.Threading.Tasks;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.map.interfaces;
using ColdMint.scripts.map.room;

namespace ColdMint.scripts.map.slotsMatcher;

public class RoomSlotsMatcher : IRoomSlotsMatcher
{
    private RoomSlot? _lastMatchedMainSlot;
    private RoomSlot? _lastMatchedMinorSlot;

    public Task<bool> IsMatch(Room? mainRoom, Room newRoom)
    {
        if (mainRoom == null)
        {
            return Task.FromResult(false);
        }

        var roomSlots = mainRoom.RoomSlots;
        if (roomSlots == null)
        {
            return Task.FromResult(false);
        }

        var newRoomSlots = newRoom.RoomSlots;
        if (newRoomSlots == null)
        {
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
                _lastMatchedMainSlot = mainRoomSlot;
                _lastMatchedMinorSlot = newRoomSlot;
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    public RoomSlot? LastMatchedMainSlot => _lastMatchedMainSlot;
    public RoomSlot? LastMatchedMinorSlot => _lastMatchedMinorSlot;
}