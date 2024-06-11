using System;

using Godot;

namespace ColdMint.scripts.item;

public readonly struct ItemType
{
    /// <summary>
    /// <para>Item id of this type</para>
    /// </summary>
    public string Id { get; init; }
    /// <summary>
    /// <para>A function returns a new item instance of this type</para>
    /// </summary>
    public Func<IItem_New> NewItemFunc { get; init; }
    /// <summary>
    /// <para>Default icon of items of this type</para>
    /// </summary>
    public Texture2D? Icon { get; init; }
    /// <summary>
    /// <para>Max number in item stack of this type</para>
    /// </summary>
    public int MaxStackQuantity { get; init; }
    /// <summary>
    /// <para>Determines how items of this type will be stacked</para>
    /// </summary>
    public StackType StackType { get; init; }
}