using System.Collections.Generic;
using ColdMint.scripts.debug;
using ColdMint.scripts.utils;
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
    /// <param name="lootList"></param>
    public static bool RegisterLootList(LootList lootList)
    {
        var id = lootList.Id;
        if (id == null)
        {
            return false;
        }

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
    public static void GenerateLootObjects(Node parentNode, LootData[] lootDataArray, Vector2 position)
    {
        if (lootDataArray.Length == 0)
        {
            return;
        }

        //Cache the loaded PackedScene object.
        //缓存已加载的PackedScene对象。
        Dictionary<string, PackedScene> packedSceneDictionary = new();
        foreach (var lootData in lootDataArray)
        {
            if (string.IsNullOrEmpty(lootData.ResPath) || lootData.Quantity == 0)
            {
                continue;
            }

            if (!packedSceneDictionary.TryGetValue(lootData.ResPath, out var packedScene))
            {
                packedScene = GD.Load<PackedScene>(lootData.ResPath);
                packedSceneDictionary.TryAdd(lootData.ResPath, packedScene);
            }

            for (var i = 0; i < lootData.Quantity; i++)
            {
                //Generate as many loot instance objects as there are loot.
                //有多少个战利品就生成多少个战利品实例对象。
                CreateLootInstanceObject(parentNode, packedScene, position);
            }
        }
    }


    /// <summary>
    /// <para>Create a loot instance object</para>
    /// <para>创建战利品实例对象</para>
    /// </summary>
    private static void CreateLootInstanceObject(Node parent, PackedScene? packedScene, Vector2 position)
    {
        if (packedScene == null)
        {
            return;
        }

        var lootObject = NodeUtils.InstantiatePackedScene<Node2D>(packedScene, parent);
        if (lootObject == null)
        {
            return;
        }

        lootObject.Position = position;
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