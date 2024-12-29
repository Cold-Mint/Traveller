namespace ColdMint.scripts.console.dynamicSuggestion;

public interface IDynamicSuggestion
{
    /// <summary>
    /// <para>Dynamic suggestions should have an ID</para>
    /// <para>动态建议应该有ID</para>
    /// </summary>
    public string ID { get; }
    
    /// <summary>
    /// <para>Gets the actual value provided by the dynamic recommendation</para>
    /// <para>获取动态建议提供的实际值</para>
    /// </summary>
    /// <returns></returns>
    public string[] GetAllSuggest();
}