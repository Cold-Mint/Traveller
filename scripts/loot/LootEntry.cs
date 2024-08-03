using System.Collections.Generic;
using System.Linq;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.loot;

/// <summary>
/// <para>Loot entry</para>
/// <para>战利品条目</para>
/// </summary>
public readonly struct LootEntry(string itemId, int minQuantity = 1, int maxQuantity = 1, int weight = 1)
{
    /// <summary>
    /// <para>ID of item</para>
    /// <para>物品ID</para>
    /// </summary>
    public string ItemId { get; init; } = itemId;

    /// <summary>
    /// <para>Minimum number of generated</para>
    /// <para>最小生成多少个</para>
    /// </summary>
    public int MinQuantity { get; init; } = minQuantity;

    /// <summary>
    /// <para>The maximum number of files to be generated</para>
    /// <para>最多生成多少个</para>
    /// </summary>
    public int MaxQuantity { get; init; } = maxQuantity;

    /// <summary>
    /// <para>Weight of probability within the drop group</para>
    /// <para>在掉落组内的生成权重</para>
    /// </summary>
    public int Weight { get; init; } = weight;
}

/// <summary>
/// <para>Loot Group</para>
/// <para>战利品分组</para>
/// </summary>
/// <param name="Chance">
///<para>Chance</para>
///<para>概率</para>
/// </param>
/// <param name="Entries">
///<para>Entries</para>
///<para>条目列表</para>
/// </param>
public readonly record struct LootGroup(float Chance, IEnumerable<LootEntry> Entries)
{
    private int WeightSum { get; } = Entries.Sum(entry => entry.Weight);

    public LootDatum GenerateLootData()
    {
        var random = RandomUtils.Instance;
        var w = random.Next(WeightSum);
        LootEntry entry = default;
        foreach (var e in Entries)
        {
            w -= e.Weight;
            if (w >= 0) continue;
            entry = e;
            break;
        }

        var quantity = random.Next(entry.MinQuantity, entry.MaxQuantity + 1);

        return new LootDatum(entry.ItemId, quantity);
    }
}