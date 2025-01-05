using System.Collections.Generic;
using System.Reflection;
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
    private readonly Dictionary<string, int> _itemTypeCodeDictionary = new();
    private readonly Dictionary<int, string> _itemTypeCodeDictionaryReverse = new();

    public void InitSuggest()
    {
        InitItemTypeCodeDictionary();
        var item = _suggest.AddChild("item");
        //Easy output of all item information
        //简易的输出全部物品信息
        AddItemType(item.AddChild("simple"));
        //Detailed output item information
        //详细的输出物品信息
        AddItemType(item.AddChild("detailed"));
        _suggest.AddChild("camp");
        _suggest.AddChild("loot_list");
        _suggest.AddChild("dynamic_suggestion");
    }

    /// <summary>
    /// <para>Initializes the item type dictionary</para>
    /// <para>初始化物品类型字典</para>
    /// </summary>
    private void InitItemTypeCodeDictionary()
    {
        var itemTypeCodeType = typeof(Config.ItemTypeCode);
        var fields = itemTypeCodeType.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            var value = field.GetValue(null);
            if (value is not int intValue)
            {
                continue;
            }

            var name = field.Name.ToLowerInvariant();
            _itemTypeCodeDictionary.Add(name, intValue);
            _itemTypeCodeDictionaryReverse.Add(intValue, name);
        }
    }

    /// <summary>
    /// <para>AddItemType</para>
    /// <para>添加物品类型</para>
    /// </summary>
    /// <param name="root"></param>
    private void AddItemType(NodeTree<string> root)
    {
        foreach (var key in _itemTypeCodeDictionary.Keys)
        {
            root.AddChild(key);
        }
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
        var itemNode = _suggest.GetChild(0);
        var item = itemNode?.Data;
        if (itemNode != null && type == item)
        {
            var showMode = args.GetString(2);
            if (string.IsNullOrEmpty(showMode))
            {
                return false;
            }

            var enableSimpleMode = true;
            var simple = itemNode.GetChild(0)?.Data;
            if (showMode == simple)
            {
                enableSimpleMode = true;
            }

            var detailed = itemNode.GetChild(1)?.Data;
            if (showMode == detailed)
            {
                enableSimpleMode = false;
            }

            var itemTypeCode = args.GetString(3);
            var itemTypeCodeInt = Config.ItemTypeCode.All;
            if (!string.IsNullOrEmpty(itemTypeCode))
            {
                itemTypeCodeInt = _itemTypeCodeDictionary[itemTypeCode];
            }

            var result = new List<string>();
            if (enableSimpleMode)
            {
                PrintArray(item, ItemTypeManager.GetAllIds(itemTypeCodeInt));
                return true;
            }

            var allItemType = ItemTypeManager.GetAllItemType();
            foreach (var itemType in allItemType)
            {
                if (itemTypeCodeInt == Config.ItemTypeCode.All)
                {
                    result.Add(PrintItemType(itemType));
                    continue;
                }

                if (itemType.TypeCode == itemTypeCodeInt)
                {
                    result.Add(PrintItemType(itemType));
                }
            }

            PrintArray(item, result.ToArray());
            return true;
        }

        var camp = _suggest.GetChild(1)?.Data;
        if (type == camp)
        {
            PrintArray(camp, CampManager.GetAllIds());
            return true;
        }

        var lootList = _suggest.GetChild(2)?.Data;
        if (type == lootList)
        {
            PrintArray(lootList, LootListManager.GetAllIds());
            return true;
        }

        var dynamicSuggestion = _suggest.GetChild(3)?.Data;
        if (type == dynamicSuggestion)
        {
            PrintArray(dynamicSuggestion, DynamicSuggestionManager.GetAllIds());
            return true;
        }

        return false;
    }

    private string PrintItemType(ItemTypeInfo itemTypeInfo)
    {
        return itemTypeInfo.Id + "|" + _itemTypeCodeDictionaryReverse[itemTypeInfo.TypeCode] + "|" +
               itemTypeInfo.ScenePath;
    }

    private void PrintArray(string type, string[] array)
    {
        ConsoleGui.Instance?.Print(TranslationServerUtils.TranslateWithFormat("log_assets_registry_echo", type,
            array.Length,
            string.Join('\n', array)));
    }
}