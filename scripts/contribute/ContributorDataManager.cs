using System;
using System.Collections.Generic;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.contribute;

/// <summary>
/// <para>Contributor data Manager</para>
/// <para>贡献者数据管理器</para>
/// </summary>
public static class ContributorDataManager
{
    private static Dictionary<ContributorType, List<ContributorData>>? _contributorTypeDictionary;

    private static readonly ContributorData[] ContributorArray =
    [
        new ContributorData
        {
            Name = "Cold-Mint",
            Url = "https://github.com/Cold-Mint",
            ContributorTypes = [ContributorType.Coder]
        },
        new ContributorData
        {
            Name = "Web13234",
            Url = "https://github.com/Web13234",
            ContributorTypes = [ContributorType.Coder]
        },
        new ContributorData
        {
            Name = "HYPERLINK BLOCKED",
            Url = "https://www.pixiv.net/users/74412798",
            ContributorTypes = [ContributorType.Artist]
        }
    ];

    /// <summary>
    /// <para>Get contributor totals</para>
    /// <para>获取贡献者总数</para>
    /// </summary>
    /// <returns></returns>
    public static int GetContributorTotals()
    {
        return ContributorArray.Length;
    }

    /// <summary>
    /// <para>Gets a dictionary of contribution types to the contributor array</para>
    /// <para>获取贡献类型到贡献者数组的字典</para>
    /// </summary>
    /// <remarks>
    ///<para>Cache the results after calling this method, as it is very expensive to generate results.</para>
    ///<para>调用此方法后请将结果缓存起来，因为生成结果是非常昂贵的。</para>
    /// </remarks>
    /// <returns>
    /// </returns>
    public static Dictionary<ContributorType, ContributorData[]>? GetContributorTypeToContributorDataArray()
    {
        if (_contributorTypeDictionary == null)
        {
            return null;
        }

        var result = new Dictionary<ContributorType, ContributorData[]>();
        foreach (var contributorType in _contributorTypeDictionary.Keys)
        {
            result[contributorType] = _contributorTypeDictionary[contributorType].ToArray();
        }

        return result;
    }

    /// <summary>
    /// <para>Register all contributor data</para>
    /// <para>注册所有的贡献者数据</para>
    /// </summary>
    public static void RegisterAllContributorData()
    {
        if (_contributorTypeDictionary!= null)
        {
            return;
        }
        foreach (var contributorData in ContributorArray)
        {
            RegisterContributorData(contributorData);
        }
    }

    /// <summary>
    /// <para>Gets a string description of a contribution type</para>
    /// <para>获取某个贡献类型的字符串描述</para>
    /// </summary>
    /// <param name="contributorType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string? ContributorTypeToString(ContributorType contributorType)
    {
        return contributorType switch
        {
            ContributorType.Coder => TranslationServerUtils.Translate("ui_coder"),
            ContributorType.Artist => TranslationServerUtils.Translate("ui_artist"),
            ContributorType.Musician => TranslationServerUtils.Translate("ui_musician"),
            ContributorType.CharacterVoice => TranslationServerUtils.Translate("ui_character_voice"),
            ContributorType.Translator => TranslationServerUtils.Translate("ui_translator"),
            _ => throw new ArgumentOutOfRangeException(nameof(contributorType), contributorType, null)
        };
    }

    /// <summary>
    /// <para>Register contributor data to the type dictionary</para>
    /// <para>注册贡献者数据到类型字典</para>
    /// </summary>
    /// <param name="contributorType"></param>
    /// <param name="contributorData"></param>
    private static void AddContributorDataToTypeDictionary(ContributorType contributorType,
        ContributorData contributorData)
    {
        if (_contributorTypeDictionary == null)
        {
            return;
        }

        if (_contributorTypeDictionary.ContainsKey(contributorType))
        {
            _contributorTypeDictionary[contributorType].Add(contributorData);
        }
        else
        {
            _contributorTypeDictionary[contributorType] = [contributorData];
        }
    }

    /// <summary>
    /// <para>Register Contributor data</para>
    /// <para>注册贡献者数据</para>
    /// </summary>
    /// <param name="contributorData"></param>
    private static void RegisterContributorData(ContributorData contributorData)
    {
        if (contributorData.Name == null || contributorData.ContributorTypes == null)
        {
            return;
        }

        _contributorTypeDictionary ??= new Dictionary<ContributorType, List<ContributorData>>();
        foreach (var contributorDataContributorType in contributorData.ContributorTypes)
        {
            AddContributorDataToTypeDictionary(contributorDataContributorType, contributorData);
        }
    }
}