using System;
using System.Collections.Generic;

using ColdMint.scripts.utils;

using Godot;

namespace ColdMint.scripts.item;

public static class ItemTypeManager
{
    // Register items statically here
    static ItemTypeManager() { }

    private static Dictionary<string, ItemType> Registry { get; } = [];
    private static Texture2D DefaultTexture { get; } = new PlaceholderTexture2D();


    /// <summary>
    /// Register a item type.
    /// Return false if the item id already exist.
    /// </summary>
    /// <returns>Whether the registration was successful.</returns>
    public static bool Register(ItemType itemType) => Registry.TryAdd(itemType.Id, itemType);

    /// <summary>
    /// <para>Creates a new instance of the item registered to the given id.</para>
    /// <para>Returns null when the id is not registered.</para>
    /// </summary>
    public static IItem_New? NewItem(string id) =>
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