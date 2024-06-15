using System;
using System.Collections.Generic;
using System.Linq;

using ColdMint.scripts.debug;
using ColdMint.scripts.utils;

using Godot;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ColdMint.scripts.item;

/// <summary>
/// 负责从文件注册物品
/// </summary>
public static class ItemTypeRegister
{
    /// <summary>
    /// <para>Register items from yaml file</para>
    /// <para>从文件注册物品</para>
    /// </summary>
    public static void RegisterFromFile()
    {
        LogCat.Log("start_item_register_from_file");

        // initialize yaml deserializer
        var deserializer = new DeserializerBuilder()
                          .WithNamingConvention(UnderscoredNamingConvention.Instance) // convent snake_case
                          .Build();

        // initialize file dir
        string itemRegsDirPath = "res://data/itemRegs/";
        var itemRegsDir = DirAccess.Open(itemRegsDirPath);
        if (DirAccess.GetOpenError() is not Error.Ok)
        {
            LogCat.LogError("error_when_open_item_regs_dir");
        }

        // find files
        var files = itemRegsDir.GetFiles();
        LogCat.LogWithFormat("found_files", files.Length);

        // parse files to item type infos
        IEnumerable<ItemTypeInfo> typeInfos =
            files.SelectMany(file => ParseFile(deserializer, $"{itemRegsDirPath}/{file}")).ToList();
        LogCat.LogWithFormat("found_item_types", typeInfos.Count());

        // traverse type infos and register them.
        foreach (var typeInfo in typeInfos)
        {
            RegisterTypeInfo(typeInfo);
        }
    }

    private static IList<ItemTypeInfo> ParseFile(IDeserializer deserializer, string filePath)
    {
        string itemRegsDirPath;
        string file;
        var yamlFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        var yamlString = yamlFile.GetAsText();
        var typeInfos = deserializer.Deserialize<IList<ItemTypeInfo>>(yamlString);
        yamlFile.Close();
        return typeInfos;
    }

    private static void RegisterTypeInfo(ItemTypeInfo typeInfo)
    {
        var scene = ResourceLoader.Load<PackedScene>(typeInfo.ScenePath);
        var icon = ResourceLoader.Load<Texture2D>(typeInfo.IconPath);
        var itemType = new ItemType(typeInfo.Id,
                                    () => NodeUtils.InstantiatePackedScene<Packsack>(scene),
                                    icon, typeInfo.MaxStackValue);
        ItemTypeManager.Register(itemType);
    }

    //Use for yaml deserialization
    private record struct ItemTypeInfo(string Id, string ScenePath, string IconPath, int MaxStackValue) { }
}