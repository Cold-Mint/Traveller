using ColdMint.scripts.inventory;

using Godot;

namespace ColdMint.scripts.item;

/// <summary>
/// <para>Item stack in inventory</para>
/// </summary>
//maybe we'd move this into inventory namespace
public readonly struct SingleItemStack(IItem_New item) : IItemStack
{
    public IItem_New Item { get; init; } = item;

    public string Id => Item.Id;
    public int MaxQuantity => 1;
    public int Quantity => 1;
    public Texture2D Icon => Item.Icon;
    public string Name => Item.Name;
    public string? Description => Item.Description;
}