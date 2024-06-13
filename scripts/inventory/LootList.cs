using System.Collections.Generic;
using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>Loot list</para>
/// <para>战利品表</para>
/// </summary>
public class LootList
{
    /// <summary>
    /// <para>Id</para>
    /// <para>战利品表的Id</para>
    /// </summary>
    public string? Id { get; set; }

    private List<LootEntry>? _lootEntrieList;

    /// <summary>
    /// <para>Add loot entry</para>
    /// <para>添加战利品条目</para>
    /// </summary>
    /// <param name="lootEntry"></param>
    public void AddLootEntry(LootEntry lootEntry)
    {
        if (_lootEntrieList == null)
        {
            _lootEntrieList = new List<LootEntry>();
        }

        _lootEntrieList.Add(lootEntry);
    }


    /// <summary>
    /// <para>GenerateLootData</para>
    /// <para>生成战利品数据</para>
    /// </summary>
    /// <returns></returns>
    public LootData[] GenerateLootData()
    {
        var lootDataList = new List<LootData>();
        if (_lootEntrieList == null)
        {
            LogCat.LogWithFormat("loot_list_has_no_entries", Id);
            return lootDataList.ToArray();
        }

        foreach (var lootEntry in _lootEntrieList)
        {
            var chance = GD.Randf();
            if (chance > lootEntry.Chance)
            {
                //If the random number is greater than the generation probability, skip the current loop.
                //如果随机数大于生成概率，则跳过当前循环。
                LogCat.LogWithFormat("not_within_the_loot_spawn_range", chance, lootEntry.ResPath, lootEntry.Chance);
                continue;
            }

            //We generate a loot data for each loot entry.
            //我们为每个战利品条目生成一个战利品数据。
            var quantity = GD.RandRange(lootEntry.MinQuantity, lootEntry.MaxQuantity);
            var lootData = new LootData
            {
                ResPath = lootEntry.ResPath,
                Quantity = quantity
            };
            lootDataList.Add(lootData);
        }

        LogCat.LogWithFormat("loot_data_quantity", lootDataList.Count);
        return lootDataList.ToArray();
    }


    /// <summary>
    /// <para>Remove loot entry</para>
    /// <para>移除战利品条目</para>
    /// </summary>
    /// <param name="lootEntry"></param>
    /// <returns></returns>
    public bool RemoveLootEntry(LootEntry lootEntry)
    {
        if (_lootEntrieList == null)
        {
            return false;
        }

        return _lootEntrieList.Remove(lootEntry);
    }
}