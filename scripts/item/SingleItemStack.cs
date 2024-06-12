using System;

using ColdMint.scripts.inventory;

using Godot;

namespace ColdMint.scripts.item;

/// <summary>
/// <para>Item stack of single item</para>
/// </summary>
//maybe we'd move this into inventory namespace
public struct SingleItemStack(IItem_New item) : IItemStack
{
    public IItem_New Item { get; init; } = item;

    public string Id => Item.Id;
    public int MaxQuantity => 1;
    public int Quantity { get; set; } = 1;
    public Texture2D Icon => Item.Icon;
    public string Name => Item.Name;
    public string? Description => Item.Description;

    public bool CanAddItem(IItem_New item) => false;

    public bool AddItem(IItem_New item) => false;

    public int CanTakeFrom(IItemStack itemStack) => 0;

    public int TakeFrom(IItemStack itemStack) => 0;

    public int RemoveItem(int number)
    {
        
    }

    public void ClearStack()
    {
        throw new System.NotImplementedException();
    }
}