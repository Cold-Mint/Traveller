using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <para>Register items here</para>
    /// <para>在这里注册物品</para>
    /// </summary>
    public static void StaticRegister()
    {
    }

    /// <summary>
    /// <para>Register items from yaml file</para>
    /// <para>从文件注册物品</para>
    /// </summary>
    public static void RegisterFromFile()
    {
        LogCat.Log("start_item_register_from_file");
        //初始化文件目录
        //initialize file dir
        var itemRegsDirPath = "res://data/itemRegs/";
        var itemRegsDir = DirAccess.Open(itemRegsDirPath);
        var error = DirAccess.GetOpenError();
        if (error is not Error.Ok)
        {
            LogCat.LogError("error_when_open_item_regs_dir",error.ToString());
        }

        //找到文件
        //find files
        var files = itemRegsDir.GetFiles();
        if (files == null)
        {
            LogCat.LogWithFormat("found_files", LogCat.LogLabel.Default, 0);
            return;
        }

        LogCat.LogWithFormat("found_files", LogCat.LogLabel.Default, files.Length);
        //将文件解析为项目类型信息
        //parse files to item type infos
        IEnumerable<ItemTypeInfo> typeInfos =
            files.SelectMany(file => ParseFile($"{itemRegsDirPath}/{file}")).ToList();
        LogCat.LogWithFormat("found_item_types", LogCat.LogLabel.Default, typeInfos.Count());

        //遍历类型信息并注册它们。
        //traverse type infos and register them.
        foreach (var typeInfo in typeInfos)
        {
            RegisterTypeInfo(typeInfo);
        }
    }

    /// <summary>
    /// <para>ParseFile</para>
    /// <para>解析文件</para>
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static IList<ItemTypeInfo> ParseFile(string filePath)
    {
        var yamlFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);

        //Read & deserialize
        //阅读和反序列化
        var yamlString = yamlFile.GetAsText();
        var typeInfos = YamlSerialization.Deserialize<IList<ItemTypeInfo>>(yamlString);
        yamlFile.Close();
        return typeInfos;
    }

    private static void RegisterTypeInfo(ItemTypeInfo typeInfo)
    {
        //Load scene and icon
        //加载场景和图标
        var scene = ResourceLoader.Load<PackedScene>(typeInfo.ScenePath);
        var icon = ResourceLoader.Load<Texture2D>(typeInfo.IconPath);


        //Create init delegate
        //创建初始化委托
        Action<Node?>? setArgs = null;
        if (typeInfo.CustomArgs != null && typeInfo.CustomArgs.Count > 0)
        {
            foreach (var arg in typeInfo.CustomArgs)
            {
                setArgs +=
                    node => node?.SetDeferred(arg.Name, arg.ParseValue());
            }
        }

        //构造项目类型，寄存器
        //construct item type, register
        var itemType = new ItemType(typeInfo.Id,
            defaultParentNode =>
            {
                var newItem = NodeUtils.InstantiatePackedScene<IItem>(scene);
                if (newItem is not Node node) return newItem;
                if (defaultParentNode == null)
                {
                    node.QueueFree();
                    return null;
                }

                setArgs?.Invoke(node);
                NodeUtils.CallDeferredAddChild(NodeUtils.FindContainerNode(node, defaultParentNode), node);
                return newItem;
            },
            icon, typeInfo.MaxStackValue);
        var succeed = ItemTypeManager.Register(itemType);
        LogCat.LogWithFormat("register_item", label: LogCat.LogLabel.Default, itemType.Id, succeed);
    }

    //Use for yaml deserialization
    //用于yaml反序列化
    private record struct ItemTypeInfo(
        string Id,
        string ScenePath,
        string IconPath,
        int MaxStackValue,
        IList<CustomArg>? CustomArgs);

    private readonly record struct CustomArg(string Name, CustomArgType Type, string Value)
    {
        public Variant ParseValue() =>
            Type switch
            {
                CustomArgType.String => Value,
                CustomArgType.Int => int.Parse(Value),
                CustomArgType.Float => double.Parse(Value),
                CustomArgType.Vector2 => ParseVector2FromString(Value),
                CustomArgType.Bool => bool.Parse(Value),
                CustomArgType.Texture => ResourceLoader.Load<Texture2D>("res://sprites/" + Value),
                _ => throw new ArgumentOutOfRangeException($"Unknown Arg Type {Type}")
            };

        private Vector2 ParseVector2FromString(string s)
        {
            var ss = s.Split(',');
            if (ss.Length != 2)
            {
                LogCat.LogErrorWithFormat("wrong_custom_arg", LogCat.LogLabel.Default, "Vector2", s);
                return Vector2.Zero;
            }

            return new Vector2(float.Parse(ss[0]), float.Parse(ss[1]));
        }
    }

    private enum CustomArgType
    {
        String,
        Int,
        Float,
        Vector2,
        Texture,
        Bool,
    }
}