using System.Linq;

namespace ColdMint.scripts.console.dynamicSuggestion;

public class ObjectSelectorDynamicSuggestion : IDynamicSuggestion
{
    public string ID => Config.DynamicSuggestionID.ObjectSelector;

    // @any 全部对象 @item 物品 @player 玩家 @character 玩家和其他生物
    public static readonly string[] AllSuggest = ["@player"];

    public bool Match(string input) => AllSuggest.Any(suggest => suggest == input);

    public string[] GetAllSuggest()
    {
        // var playerName = GameSceneDepend.Player?.CharacterName;
        // List<string> resultList = [];
        // resultList.AddRange(AllSuggest);
        // if (!string.IsNullOrEmpty(playerName))
        // {
        //     resultList.Add(playerName);
        // }
        // return resultList.ToArray();
        return AllSuggest;
    }
}