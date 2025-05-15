using ColdMint.scripts.console.dynamicSuggestion;

namespace ColdMint.scripts.console.objectSelector;

/// <summary>
/// <para>Object Selector Query Request</para>
/// <para>对象选择器查询请求</para>
/// </summary>
public class ObjectSelectorQueryRequest
{
    /// <summary>
    /// <para>The type of object to be queried</para>
    /// <para>要查询的对象类型</para>
    /// </summary>
    public int Type = Config.ObjectType.All;

    private const string Prefix = "@";

    /// <summary>
    /// <para>Parse</para>
    /// <para>解析字符串</para>
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static ObjectSelectorQueryRequest? Parse(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        var request = new ObjectSelectorQueryRequest();
        if (str.StartsWith(Prefix))
        {
            //Generic matching
            //泛型匹配
            if (str == ObjectSelectorDynamicSuggestion.AllSuggest[0])
            {
                request.Type = Config.ObjectType.Player;
            }
        }

        return request;
    }
}