using System.Linq;
using ColdMint.scripts.map;

namespace ColdMint.scripts.console.dynamicSuggestion;

/// <summary>
/// <para>Room Dynamic Suggestion</para>
/// <para>房间建议</para>
/// </summary>
public class RoomDynamicSuggestion : IDynamicSuggestion
{
    public string ID => Config.DynamicSuggestionID.Room;

    public bool Match(string input)
    {
        var roomList = MapGenerator.GetRoomList();
        return roomList.Any(roomId => roomId == input);
    }

    public string[] GetAllSuggest()
    {
        return MapGenerator.GetRoomList();
    }
}