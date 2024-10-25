using System;
using System.Collections.Generic;
using ColdMint.scripts.debug;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.loot;

/// <summary>
/// <para>Loot list</para>
/// <para>战利品表</para>
/// </summary>
public class LootList
{

    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public string? Id { get; set; }

    public IList<LootGroup>? Groups { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global


    private static Random Random => RandomUtils.Instance;

    /// <summary>
    /// <para>GenerateLootData</para>
    /// <para>生成战利品数据</para>
    /// </summary>
    /// <returns></returns>
    public LootDatum[] GenerateLootData()
    {
        if (Id == null || Groups == null)
        {
            return [];
        }
        if (Groups is [])
        {
            LogCat.LogWithFormat("loot_list_has_no_entries", LogCat.LogLabel.Default, Id);
            return [];
        }

        var lootDataList = new List<LootDatum>();
        foreach (var group in Groups)
        {
            //If the random number is greater than the generation probability, skip the current loop.
            //如果随机数大于生成概率，则跳过当前循环。
            var rd = Random.NextDouble();
            if (rd > group.Chance) continue;

            //We generate a loot data for each loot entry.
            //我们为每个战利品条目生成一个战利品数据。
            var datum = group.GenerateLootData();
            lootDataList.Add(datum);
            LogCat.LogWithFormat("loot_data_add", LogCat.LogLabel.Default, datum);
        }

        LogCat.LogWithFormat("loot_data_quantity", LogCat.LogLabel.Default, lootDataList.Count);
        return lootDataList.ToArray();
    }
}