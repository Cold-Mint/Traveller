using System.Collections.Generic;
using ColdMint.scripts.inventory;
using Godot;

namespace ColdMint.scripts.loot;

/// <summary>
/// <para>LootListManager</para>
/// <para>战利品表管理器</para>
/// </summary>
public static class LootListManager
{
    private static Dictionary<string, LootList> LootListDictionary { get; } = [];

    /// <summary>
    /// <para>Register loot table</para>
    /// <para>注册战利品表</para>
    /// </summary>
    /// <param name="lootList"></param>
    public static bool RegisterLootList(LootList lootList)
    {
        var id = lootList.Id;
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }
        return LootListDictionary.TryAdd(id, lootList);
    }

    /// <summary>
    /// <para>Remove loot list</para>
    /// <para>移除战利品表</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool UnregisterLootList(string id)
    {
        return LootListDictionary.Remove(id);
    }

    /// <summary>
    /// <para>Generate an loot data.</para>
    /// <para>获取掉落物品</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static IEnumerable<LootDatum> GenerateLootData(string id)
    {
        if (!LootListDictionary.TryGetValue(id, out var list)) return [];
        return list.GenerateLootData();
    }

    /// <summary>
    /// <para>GenerateLootObjects</para>
    /// <para>生成战利品对象</para>
    /// </summary>
    /// <param name="parentNode"></param>
    /// <param name="lootData"></param>
    /// <param name="position"></param>
    public static void GenerateLootObjects(Node parentNode,
        IEnumerable<LootDatum> lootData,
        Vector2 position)
    {
        foreach (var lootDatum in lootData)
        {
            var (id, amount) = lootDatum.Value;
            ItemTypeManager.CreateItems(id, amount, position, parentNode);
        }
    }
}