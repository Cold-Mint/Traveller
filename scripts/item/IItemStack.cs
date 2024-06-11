using System;

using Godot;

namespace ColdMint.scripts.item;

public interface IItemStack
{
    /// <summary>
    /// <para>ID of items inside current stack</para>
    /// </summary>
    string Id { get; }

    /// <summary>
    /// <para>Max number of current stack</para>
    /// </summary>
    int MaxQuantity { get; }

    /// <summary>
    /// <para>Quantity of current stack</para>
    /// </summary>
    int Quantity { get; }

    /// <summary>
    /// <para>Icon of current item</para>
    /// </summary>
    Texture2D Icon { get; }

    /// <summary>
    /// <para>Display name of current item</para>
    /// </summary>
    string Name { get; }

    /// <summary>
    /// <para>Description of current item, which may show in inventory</para>
    /// </summary>
    string? Description { get; }


    /// <summary>
    /// Create a new ItemStack with the given item as the first item
    /// </summary>
    public static IItemStack FromItem(IItem_New item) => ItemTypeManager.MaxStackQuantityOf(item.Id) switch
    {
        1         => new SingleItemStack(item),
        > 1       => throw new NotImplementedException(),
        var other => throw new ArgumentException($"item {item} of type '{item.Id}' has unexpected max stack quantity {other}")
    };
}