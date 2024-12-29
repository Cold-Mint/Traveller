using System.Threading.Tasks;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.console.commands;

public class PlayerCommand : ICommand
{
    public string Name => Config.CommandNames.Player;
    private readonly NodeTree<string> _suggest = new(null);

    public string[] GetAllSuggest(CommandArgs args)
    {
        return SuggestUtils.GetAllSuggest(args, _suggest);
    }

    public void InitSuggest()
    {
        var indestructible = _suggest.AddChild("indestructible");
        indestructible.AddChild(
            DynamicSuggestionManager.CreateDynamicSuggestionReferenceId(Config.DynamicSuggestionID.Boolean));
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
        var indestructible = _suggest.GetChild(0)?.Data;
        if (type == indestructible)
        {
            if (args.Length < 3)
            {
                return Task.FromResult(false);
            }

            var player = GameSceneDepend.Player;
            if (player == null)
            {
                return Task.FromResult(false);
            }

            var value = args.GetBool(2);
            player.Indestructible = value;
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}