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
        });
    }

    private static List<LootList>? ParseFile(string filePath)
    {
        var yamlFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        var lootLists = YamlSerialization.Deserialize<List<LootList>>(yamlFile.GetAsText());
        yamlFile.Close();
        return lootLists;
    }
}