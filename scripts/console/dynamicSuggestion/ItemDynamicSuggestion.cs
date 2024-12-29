using ColdMint.scripts.inventory;

namespace ColdMint.scripts.console.dynamicSuggestion;

public class ItemDynamicSuggestion : IDynamicSuggestion
{
    public string ID => Config.DynamicSuggestionID.Item;

    public string[] GetAllSuggest()
    {
        return ItemTypeManager.GetAllIds();
    }
}