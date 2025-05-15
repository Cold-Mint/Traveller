using System.Linq;

namespace ColdMint.scripts.console.dynamicSuggestion;

public class ObjectSelectorDynamicSuggestion : IDynamicSuggestion
{
    public string ID => Config.DynamicSuggestionID.ObjectSelector;
    private readonly string[] _allSuggest = ["@p"];

    public bool Match(string input) => _allSuggest.Any(suggest => suggest == input);

    public string[] GetAllSuggest()
    {
        return _allSuggest;
    }
}