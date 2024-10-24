using System.Collections.Generic;
using ColdMint.scripts.debug;
using ColdMint.scripts.inventory;
using ColdMint.scripts.serialization;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.loot;

public static class LootRegister
{

    /// <summary>
    /// <para></para>
    /// <para>从文件系统注册战利品</para>
    /// </summary>
    public static void RegisterFromFile()
    {
        const string lootPath = "res://data/loots";
        ResUtils.ScanResDirectory(lootPath, s =>
        {
            //TODO:实现战利品表的加载。
        });
    }

    private static List<LootList>? ParseFile(string filePath)
    {
        var yamlFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        //Read & deserialize
        //阅读和反序列化
        var yamlString = yamlFile.GetAsText();
        var lootLists = YamlSerialization.Deserialize<List<LootList>>(yamlString);
        yamlFile.Close();
        return lootLists;
    }

    /// <summary>
    /// <para>Register loots hardcoded here</para>
    /// <para>在这里硬编码地注册掉落表</para>
    /// </summary>
    public static void StaticRegister()
    {
        //Register the test using the loot table
        //注册测试使用的战利品表
        if (Config.IsDebug())
        {
            List<LootGroup> lootGroups =
            [
                new LootGroup(0.8f,
                [
                    new LootEntry("staff_necromancy"),
                ]),
                new LootGroup(1, [new LootEntry("portable_backpacks")])
            ];
            var testLootList = new LootList(Config.LootListId.Test, lootGroups);
            LootListManager.RegisterLootList(testLootList);
        }
    }
}