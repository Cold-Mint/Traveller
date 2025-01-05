using System.Threading.Tasks;
using ColdMint.scripts.map;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.console.commands;

/// <summary>
/// <para>RoomCommand</para>
/// <para>房间命令</para>
/// </summary>
public class RoomCommand : ICommand
{
    public string Name => Config.CommandNames.Room;
    private readonly NodeTree<string> _suggest = new(null);

    public string[] GetAllSuggest(CommandArgs args)
    {
        return SuggestUtils.GetAllSuggest(args, _suggest);
    }

    public void InitSuggest()
    {
        _suggest.AddChild("list");
        var tp = _suggest.AddChild("tp");
        tp.AddChild(DynamicSuggestionManager.CreateDynamicSuggestionReferenceId(Config.DynamicSuggestionID.Room));
    }

    public Task<bool> Execute(CommandArgs args)
    {
        if (args.Length < 2)
        {
            return Task.FromResult(false);
        }

        var inputType = args.GetString(1);
        if (string.IsNullOrEmpty(inputType))
        {
            return Task.FromResult(false);
        }

        var type = inputType.ToLowerInvariant();
        var list = _suggest.GetChild(0)?.Data;
        if (type == list)
        {
            var roomList = MapGenerator.GetRoomList();
            ConsoleGui.Instance?.Print(TranslationServerUtils.TranslateWithFormat("log_rooms_echo", roomList.Length,
                string.Join('\n', roomList)));
            return Task.FromResult(true);
        }

        var tp = _suggest.GetChild(1)?.Data;
        if (type == tp)
        {
            var player = GameSceneDepend.Player;
            if (player == null)
            {
                return Task.FromResult(false);
            }

            var roomId = args.GetString(2);
            if (string.IsNullOrEmpty(roomId))
            {
                return Task.FromResult(false);
            }

            var room = MapGenerator.GetRoom(roomId);
            if (room == null)
            {
                return Task.FromResult(false);
            }

            var playerTransportPoint = room.PlayerTransportPoint;
            if (playerTransportPoint == null)
            {
                return Task.FromResult(false);
            }

            player.GlobalPosition = playerTransportPoint.GlobalPosition;
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}