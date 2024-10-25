using System.Collections.Generic;
using ColdMint.scripts.debug;
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
        LogCat.Log("start_loot_register_from_file");
        const string lootPath = "res://data/loots";
        var count = 0;
        ResUtils.ScanResDirectory(lootPath, s =>
        {
            var lootList = ParseFile(s);
            if (lootList == null)
            {
                return;
            }
            foreach (var list in lootList)
            {
                LootListManager.RegisterLootList(list);
            }
            count++;
        });
        LogCat.LogWithFormat("found_loots" ,LogCat.LogLabel.Default, count);
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
}