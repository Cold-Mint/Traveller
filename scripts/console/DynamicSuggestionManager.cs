using System.Collections.Generic;
using System.Linq;
using ColdMint.scripts.console.dynamicSuggestion;

namespace ColdMint.scripts.console;

public static class DynamicSuggestionManager
{
    private static readonly Dictionary<string, IDynamicSuggestion> SuggestionDict = new();

    private const string Prefix = "@";


    /// <summary>
    /// <para>Register dynamic suggestion</para>
    /// <para>注册动态建议</para>
    /// </summary>
    /// <param name="suggestion"></param>
    public static void RegisterDynamicSuggestion(IDynamicSuggestion suggestion) =>
        SuggestionDict.Add(CreateDynamicSuggestionReferenceId(suggestion.ID), suggestion);

    /// <summary>
    /// <para>Get dynamic suggestions</para>
    /// <para>获取动态建议</para>
    /// </summary>
    /// <param name="referenceId">
    ///<para>Must be prefixed!</para>
    ///<para>必须带有前缀！</para>
    /// </param>
    /// <returns></returns>
    public static IDynamicSuggestion? GetDynamicSuggestion(string referenceId) =>
        SuggestionDict.GetValueOrDefault(referenceId);


    /// <summary>
    /// <para>Create Dynamic suggestion Reference ID</para>
    /// <para>创建动态建议的引用Id</para>
    /// </summary>
    /// <remarks>
    ///<para>For example, convert: Boolean to @Boolean</para>
    ///<para>例如将：Boolean转为@Boolean</para>
    /// </remarks>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string CreateDynamicSuggestionReferenceId(string id) => Prefix + id;

    public static void UnRegisterDynamicSuggestion(string id) => SuggestionDict.Remove(id);

    public static string[] GetAllIds() => SuggestionDict.Keys.ToArray();
}