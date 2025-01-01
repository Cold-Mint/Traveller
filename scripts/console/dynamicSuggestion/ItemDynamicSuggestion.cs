using ColdMint.scripts.inventory;

namespace ColdMint.scripts.console.dynamicSuggestion;

public class ItemDynamicSuggestion : IDynamicSuggestion
{
    public string ID => Config.DynamicSuggestionID.Item;

    public bool Match(string input)
    {
        return ItemTypeManager.Contains(input);
    }

    public string[] GetAllSuggest()
    {
        return ItemTypeManager.GetAllIds();
    }
}