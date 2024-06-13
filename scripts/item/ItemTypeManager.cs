using System.Collections.Generic;
using System.Linq;

using ColdMint.scripts.debug;
using ColdMint.scripts.utils;

using Godot;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ColdMint.scripts.item;

public static class ItemTypeManager
{
    //Use for yaml deserialization
    private record struct ItemTypeInfo(string Id, string ScenePath, string IconPath, int MaxStackValue) { }

    /// <summary>
    /// Register items from yaml file
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

        // traverse the dir, find files to register
        foreach (var file in itemRegsDir.GetFiles())
        {
            if (file is null) continue;
            LogCat.LogWithFormat("item_register_from_file", file);

            // read file, parse to an IEnumerable of type infos
            var yamlFile = FileAccess.Open($"{itemRegsDirPath}/{file}", FileAccess.ModeFlags.Read);
            var yamlString = yamlFile.GetAsText();
            var typeInfos = deserializer.Deserialize<IEnumerable<ItemTypeInfo>>(yamlString);
            yamlFile.Close();

            // traverse type infos and register them.
            foreach (var typeInfo in typeInfos)
            {
                LogCat.LogWithFormat("item_register_find_item_in_file", typeInfo.Id);
                var scene = ResourceLoader.Load<PackedScene>(typeInfo.ScenePath);
                var icon = ResourceLoader.Load<Texture2D>(typeInfo.IconPath);
                var itemType = new ItemType(typeInfo.Id, () => scene.Instantiate<IItem>(), icon, typeInfo.MaxStackValue);
                Register(itemType);
            }
        }
    }

    /// <summary>
    /// Register items statically here
    /// </summary>
    public static void StaticRegister()
    {
        
    }

    private static Dictionary<string, ItemType> Registry { get; } = [];
    private static Texture2D DefaultTexture { get; } = new PlaceholderTexture2D();


    /// <summary>
    /// Register an item type.
    /// Return false if the item id already exist.
    /// </summary>
    /// <returns>Whether the registration was successful.</returns>
    public static bool Register(ItemType itemType) => Registry.TryAdd(itemType.Id, itemType);

    /// <summary>
    /// <para>Creates a new instance of the item registered to the given id.</para>
    /// <para>Returns null when the id is not registered.</para>
    /// </summary>
    public static IItem? NewItem(string id) =>
        Registry.TryGetValue(id, out var itemType) ? itemType.NewItemFunc() : null;

    /// <summary>
    /// Get the translated default name of the item type for the given id
    /// </summary>
    /// <returns>
    /// Translated default name of the item id if it exists. Else, return the id itself
    /// </returns>
    public static string DefaultNameOf(string id) => TranslationServerUtils.Translate($"item_{id}") ?? id;

    /// <summary>
    /// Get the translated default description of the item type for the given id
    /// </summary>
    /// <returns>
    /// Translated default description of the item id if it exists. Else, return null
    /// </returns>
    public static string? DefaultDescriptionOf(string id) => TranslationServerUtils.Translate($"item_{id}_desc");

    /// <summary>
    /// Get the default icon of the item type for the given id
    /// </summary>
    /// <returns>
    /// Translated default icon of the item id if it exists. Else, return a placeholder
    /// </returns>
    public static Texture2D DefaultIconOf(string id) =>
        Registry.TryGetValue(id, out var itemType)
            ? itemType.Icon ?? DefaultTexture
            : DefaultTexture;

    public static int MaxStackQuantityOf(string id) => Registry.TryGetValue(id, out var itemType) ? itemType.MaxStackQuantity : 0;
}