using System.Collections.Generic;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>LootListManager</para>
/// <para>战利品表管理器</para>
/// </summary>
public static class LootListManager
{
    private static Dictionary<string, LootList>? _lootListDictionary;


    /// <summary>
    /// <para>Register loot table</para>
    /// <para>注册战利品表</para>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="lootList"></param>
    public static bool RegisterLootList(string id, LootList lootList)
    {
        if (_lootListDictionary != null) return _lootListDictionary.TryAdd(id, lootList);
        _lootListDictionary = new Dictionary<string, LootList> { { id, lootList } };
        return true;
    }

    /// <summary>
    /// <para>Get Loot List</para>
    /// <para>获取战利品表</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static LootList? GetLootList(string id)
    {
        return _lootListDictionary?.GetValueOrDefault(id);
    }

    /// <summary>
    /// <para>Generate loot objects</para>
    /// <para>生成战利品对象</para>
    /// </summary>
    /// <param name="lootDataArray">
    ///<para>lootDataArray</para>
    ///<para>战利品数组</para>
    /// </param>
    /// <param name="parentNode">
    ///<para>parentNode</para>
    ///<para>父节点</para>
    /// </param>
    public static void GenerateLootObjects(LootData[] lootDataArray, Node parentNode)
    {
        if (lootDataArray.Length == 0)
        {
            return;
        }

        Dictionary<string, PackedScene> packedSceneDictionary = new();
        foreach (var lootData in lootDataArray)
        {
            if (string.IsNullOrEmpty(lootData.ResPath))
            {
                continue;
            }

            if (!packedSceneDictionary.TryGetValue(lootData.ResPath, out var packedScene))
            {
                packedScene = GD.Load<PackedScene>(lootData.ResPath);
                packedSceneDictionary.TryAdd(lootData.ResPath, packedScene);
            }

            CreateLootObject(packedScene, parentNode);
        }
    }

    private static void CreateLootObject(PackedScene? packedScene, Node parent)
    {
        if (packedScene == null)
        {
            return;
        }

        var lootObject = packedScene.Instantiate();
        if (lootObject is not Node2D node2D)
        {
            return;
        }

        parent.AddChild(node2D);
    }

    /// <summary>
    /// <para>Remove loot list</para>
    /// <para>移除战利品表</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool UnregisterLootList(string id)
    {
        if (_lootListDictionary == null)
        {
            return false;
        }

        return _lootListDictionary.Remove(id);
    }
}