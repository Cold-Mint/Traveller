using System;
using System.Collections.Generic;
using System.Linq;
using ColdMint.scripts.console;

namespace ColdMint.scripts.utils;

public static class SuggestUtils
{
    /// <summary>
    /// <para>Obtain the prompt based on the entered command parameters in the node tree.</para>
    /// <para>在节点树内根据已输入的命令参数获取对应的提示。</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="rootNode"></param>
    /// <returns></returns>
    public static string[] GetAllSuggest(CommandArgs args, NodeTree<string> rootNode)
    {
        string[] emptyArray = [];
        if (args.Length <= 1)
        {
            return emptyArray;
        }

        //Start the loop with element 1, because we want to remove the command name.
        //从1号元素开始循环，因为我们要去除命令名。
        var nextNode = rootNode;
        for (var i = 1; i < args.Length; i++)
        {
            var input = args.GetString(i);
            if (input == null)
            {
                continue;
            }

            var newNode = nextNode.GetChildByValue(input);
            if (newNode == null)
            {
                return nextNode.GetAllChildren()?? emptyArray;
            }
            nextNode = newNode;
        }

        var resultFromNode = nextNode.GetAllChildren();
        if (resultFromNode == null)
        {
            return emptyArray;
        }

        return resultFromNode;
    }

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
    public static AutoCompleteSuggestion[] ScreeningSuggestion(IEnumerable<string> allSuggest, string? keyword,
        bool enableBbCode = true)
    {
        List<AutoCompleteSuggestion> result = [];
        if (string.IsNullOrEmpty(keyword))
        {
            result.AddRange(allSuggest.Select(se => new AutoCompleteSuggestion(se, se)));
            return result.ToArray();
        }

        var lowerKeyword = keyword.ToLowerInvariant();

        foreach (var suggest in allSuggest)
        {
            var lowerSuggest = suggest.ToLowerInvariant();
            if (lowerSuggest.StartsWith(lowerKeyword))
            {
                result.Insert(0,
                    new AutoCompleteSuggestion(enableBbCode ? RenderKeyword(suggest, keyword) : suggest, suggest));
                continue;
            }

            if (lowerSuggest.Contains(lowerKeyword))
            {
                result.Add(
                    new AutoCompleteSuggestion(enableBbCode ? RenderKeyword(suggest, keyword) : suggest, suggest));
            }
        }

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
            return suggest[..startIndex] + "[color=aqua]" + suggest[startIndex..] + "[/color]";
        }

        return suggest[..startIndex] + "[color=aqua]" + suggest.Substring(startIndex, keyword.Length) + "[/color]" +
               suggest[endIndex..];
    }
}