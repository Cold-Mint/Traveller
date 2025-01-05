using System.Collections.Generic;
using ColdMint.scripts.debug;
using ColdMint.scripts.serialization;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// Responsible for registering items from documents
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
        //初始化文件目录
        //initialize file dir
        const string itemRegsDirPath = "res://data/itemRegs";
        //将文件解析为项目类型信息
        //parse files to item type infos
        ResUtils.ScanResDirectory(itemRegsDirPath, s =>
        {
            var list = ParseFile(s);
            if (list == null)
            {
                return;
            }

            foreach (var itemTypeInfo in list)
            {
                RegisterTypeInfo(itemTypeInfo);
            }
        });
    }

    /// <summary>
    /// <para>ParseFile</para>
    /// <para>解析文件</para>
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static IList<ItemTypeInfo>? ParseFile(string filePath)
    {
        var yamlFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        //Read & deserialize
        //阅读和反序列化
        var typeInfos = YamlSerialization.Deserialize<IList<ItemTypeInfo>>(yamlFile.GetAsText());
        yamlFile.Close();
        return typeInfos;
    }

    /// <summary>
    /// <para>Registration type info</para>
    /// <para>注册类型信息</para>
    /// </summary>
    /// <param name="typeInfo">
    ///<para>typeInfo</para>
    ///<para>类型信息</para>
    /// </param>
    private static void RegisterTypeInfo(ItemTypeInfo typeInfo)
    {
        var succeed = ItemTypeManager.Register(typeInfo);
        LogCat.LogWithFormat("register_item", label: LogCat.LogLabel.Default, typeInfo.Id,
            succeed);
    }
}