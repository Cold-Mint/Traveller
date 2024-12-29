using System.Threading.Tasks;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.console.commands;

public class FogCommand : ICommand
{
    public string Name => Config.CommandNames.Fog;
    private readonly NodeTree<string> _suggest = new(null);

    public string[] GetAllSuggest(CommandArgs args)
    {
        return SuggestUtils.GetAllSuggest(args, _suggest);
    }

    public void InitSuggest()
    {
        var visible = _suggest.AddChild("visible");
        visible.AddChild(
            DynamicSuggestionManager.CreateDynamicSuggestionReferenceId(Config.DynamicSuggestionID.Boolean));
    }

    public async Task<bool> Execute(CommandArgs args)
    {
        if (args.Length < 2)
        {
            return false;
        }

        var inputType = args.GetString(1);
        if (string.IsNullOrEmpty(inputType))
        {
            return false;
        }

        var type = inputType.ToLowerInvariant();
        var visible = _suggest.GetChild(0)?.Data;
        if (type == visible)
        {
            var fog = GameSceneDepend.Fog;
            if (fog == null)
            {
                return false;
            }

            fog.Visible = args.GetBool(2);
            return true;
        }

        return false;
    }
}