using System.Threading.Tasks;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.console.commands;

public class DebugCommand : ICommand
{
    private readonly NodeTree<string> _suggest = new(null);
    public string Name => Config.CommandNames.Debug;

    public string[] GetAllSuggest(CommandArgs args)
    {
        return SuggestUtils.GetAllSuggest(args, _suggest);
    }

    public void InitSuggest()
    {
        var showObjectDetails = _suggest.AddChild("show_object_details");
        showObjectDetails.AddChild(
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
        var show = _suggest.GetChild(0)?.Data;
        if (type == show)
        {
            GameSceneDepend.ShowObjectDetails = args.GetBool(2);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}