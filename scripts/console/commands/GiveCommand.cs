using System.Threading.Tasks;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.console.commands;

public class GiveCommand : ICommand
{
    public string Name => Config.CommandNames.Give;
    private readonly NodeTree<string> _suggest = new(null);

    public string[] GetAllSuggest(CommandArgs args)
    {
        return SuggestUtils.GetAllSuggest(args, _suggest);
    }

    public void InitSuggest()
    {
        var objectSelectorNode =
            _suggest.AddChild(
                DynamicSuggestionManager.CreateDynamicSuggestionReferenceId(Config.DynamicSuggestionID.ObjectSelector));
        objectSelectorNode.AddChild(
            DynamicSuggestionManager.CreateDynamicSuggestionReferenceId(Config.DynamicSuggestionID.Item));
    }

    public Task<bool> Execute(CommandArgs args)
    {
        return Task.FromResult(false);
    }
}