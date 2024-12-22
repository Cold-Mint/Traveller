using System;
using System.Collections.Generic;
using System.Linq;

namespace ColdMint.scripts.utils;

public static class SuggestUtils
{
    /// <summary>
    /// <para>In all suggestions, filter suggestions by keyword</para>
    /// <para>在全部的建议内，按照关键字筛选建议</para>
    /// </summary>
    /// <param name="allSuggest">
    ///<para>allSuggest</para>
    ///<para>全部建议</para>
    /// </param>
    /// <param name="keyword">
    ///<para>keyword</para>
    ///<para>关键字</para>
    /// </param>
    /// <param name="enableBbCode">
    ///<para>enableBbCode</para>
    ///<para>启用BbCode</para>
    /// </param>
    /// <returns></returns>
    public static string[] ScreeningSuggestion(IEnumerable<string> allSuggest, string? keyword,
        bool enableBbCode = true)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            return allSuggest.ToArray();
        }

        var lowerKeyword = keyword.ToLowerInvariant();
        List<string> result = [];
        foreach (var suggest in allSuggest)
        {
            var lowerSuggest = suggest.ToLowerInvariant();
            if (lowerSuggest.StartsWith(lowerKeyword))
            {
                result.Insert(0, enableBbCode ? RenderKeyword(suggest, keyword) : suggest);
                continue;
            }

            if (lowerSuggest.Contains(lowerKeyword))
            {
                result.Add(enableBbCode ? RenderKeyword(suggest, keyword) : suggest);
            }
        }

        // LogCat.Log("关键字" + keyword + "列表为" + YamlSerialization.Serialize(allSuggest) + "建议为" +
                   // YamlSerialization.Serialize(result));
        return result.ToArray();
    }

    /// <summary>
    /// <para>RenderKeyword</para>
    /// <para>渲染关键字</para>
    /// </summary>
    /// <param name="suggest"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    private static string RenderKeyword(string suggest, string keyword)
    {
        var suggestWords = suggest.ToLowerInvariant();
        var lowerKeyword = keyword.ToLowerInvariant();
        var startIndex = suggestWords.IndexOf(lowerKeyword, StringComparison.Ordinal);
        if (startIndex == -1)
        {
            return suggest;
        }

        var endIndex = startIndex + keyword.Length;
        if (endIndex > suggest.Length)
        {
            return suggest[..startIndex] + "[color=green]" + suggest[startIndex..] + "[/color]";
        }

        return suggest[..startIndex] + "[color=green]" + suggest.Substring(startIndex, endIndex) + "[/color]" +
               suggest[endIndex..];
    }
}