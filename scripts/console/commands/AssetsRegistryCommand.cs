using System.Threading.Tasks;
using ColdMint.scripts.camp;
using ColdMint.scripts.inventory;
using ColdMint.scripts.loot;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.console.commands;

/// <summary>
/// <para>The AssetsRegistry command is used to list the registered assets in the game</para>
/// <para>AssetsRegistry命令用于列出游戏内已注册的资产</para>
/// </summary>
public class AssetsRegistryCommand : ICommand
{
    public string Name => Config.CommandNames.AssetsRegistry;
    private readonly NodeTree<string> _suggest = new(null);
    public string[] GetAllSuggest(CommandArgs args) => SuggestUtils.GetAllSuggest(args, _suggest);

    public void InitSuggest()
    {
        _suggest.AddChild("item");
        _suggest.AddChild("camp");
        _suggest.AddChild("loot_list");
        _suggest.AddChild("dynamic_suggestion");
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
        var item = _suggest.GetChild(0)?.Data;
        if (type == item)
        {
            Echo(item, ItemTypeManager.GetAllIds());
            return true;
        }

        var camp = _suggest.GetChild(1)?.Data;
        if (type == camp)
        {
            Echo(camp, CampManager.GetAllIds());
            return true;
        }
        var lootList = _suggest.GetChild(2)?.Data;
        if (type == lootList)
        {
            Echo(lootList, LootListManager.GetAllIds());
            return true;
        }
        var dynamicSuggestion = _suggest.GetChild(3)?.Data;
        if (type == dynamicSuggestion)
        {
            Echo(dynamicSuggestion, DynamicSuggestionManager.GetAllIds());
            return true;
        }
        return false;
    }

    private void Echo(string type, string[] array)
    {
        var msg = TranslationServerUtils.TranslateWithFormat("log_assets_registry_echo", type, array.Length,
            string.Join(',', array));
        if (msg == null)
        {
            return;
        }

        ConsoleGui.Instance?.Echo(msg);
    }
}