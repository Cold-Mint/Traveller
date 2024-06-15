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
        var yamlFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        //Read & deserialize
        var yamlString = yamlFile.GetAsText();
        var typeInfos = deserializer.Deserialize<IList<ItemTypeInfo>>(yamlString);
        
        yamlFile.Close();
        return typeInfos;
    }

    private static void RegisterTypeInfo(ItemTypeInfo typeInfo)
    {
        //Load scene and icon
        var scene = ResourceLoader.Load<PackedScene>(typeInfo.ScenePath);
        var icon = ResourceLoader.Load<Texture2D>(typeInfo.IconPath);
        
        //Create init delegate
        Func<IItem?> newItemFunc;
        if (typeInfo.CustomArgs is null or [])
        {
            newItemFunc = () => NodeUtils.InstantiatePackedScene<IItem>(scene);
        }
        else
        {
            Action<Node?>? setArgs = null;
            foreach (var arg in typeInfo.CustomArgs)
            {
                setArgs +=
                    node => node?.SetDeferred(arg.Name, arg.ParseValue());
            }

            newItemFunc = () =>
            {
                var newItem = NodeUtils.InstantiatePackedScene<IItem>(scene);
                setArgs?.Invoke(newItem as Node);
                return newItem;
            };
        }

        //construct item type, register
        var itemType = new ItemType(typeInfo.Id,
                                    newItemFunc,
                                    icon, typeInfo.MaxStackValue);
        ItemTypeManager.Register(itemType);
    }

    //Use for yaml deserialization
    private record struct ItemTypeInfo(
        string Id, string ScenePath, string IconPath, int MaxStackValue,
        IList<CustomArg>? CustomArgs) { }

    private readonly record struct CustomArg(string Name, CustomArgType Type, string Value)
    {
        public Variant ParseValue() =>
            Type switch
            {
                CustomArgType.String  => Value,
                CustomArgType.Int     => int.Parse(Value),
                CustomArgType.Float   => double.Parse(Value),
                CustomArgType.Vector2 => ParseVector2FromString(Value),
                CustomArgType.Texture => ResourceLoader.Load<Texture2D>("res://sprites/" + Value),
                _                     => throw new ArgumentOutOfRangeException($"Unknown Arg Type {Type}")
            };

        private Vector2 ParseVector2FromString(string s)
        {
            var ss = s.Split(',');
            if (ss.Length != 2)
            {
                LogCat.LogErrorWithFormat("wrong_custom_arg", "Vector2", s);
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
    }
}