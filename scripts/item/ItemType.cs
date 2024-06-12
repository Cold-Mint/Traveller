using System;

using Godot;

namespace ColdMint.scripts.item;

public readonly struct ItemType(string id, Func<IItem> newItemFunc, Texture2D? icon, int maxStackQuantity)
{
    /// <summary>
    /// <para>Item id of this type</para>
    /// </summary>
    public string Id { get; init; } = id;
    /// <summary>
    /// <para>A function returns a new item instance of this type</para>
    /// </summary>
    public Func<IItem> NewItemFunc { get; init; } = newItemFunc;
    /// <summary>
    /// <para>Default icon of items of this type</para>
    /// </summary>
    public Texture2D? Icon { get; init; } = icon;
    /// <summary>
    /// <para>Max number in item stack of this type</para>
    /// </summary>
    public int MaxStackQuantity { get; init; } = maxStackQuantity;
}