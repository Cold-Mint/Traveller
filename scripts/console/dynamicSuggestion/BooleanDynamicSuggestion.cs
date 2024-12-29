namespace ColdMint.scripts.console.dynamicSuggestion;

/// <summary>
/// <para>Gets a logical dynamic suggestion value</para>
/// <para>获取逻辑动态建议值</para>
/// </summary>
public class BooleanDynamicSuggestion : IDynamicSuggestion
{
    public string ID => Config.DynamicSuggestionID.Boolean;

    public string[] GetAllSuggest()
    {
        return ["true", "false"];
    }
}