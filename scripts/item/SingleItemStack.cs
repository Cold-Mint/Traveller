using System;

using ColdMint.scripts.inventory;

using Godot;

namespace ColdMint.scripts.item;

/// <summary>
/// <para>Item stack of single item</para>
/// </summary>
//maybe we'd move this into inventory namespace
public class SingleItemStack(IItem_New item) : IItemStack
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

    public bool TakeFrom(IItemStack itemStack) => false;

    public IItem_New? GetItem()
    {
        throw new NotImplementedException();
    }

    public IItem_New? PickItem()
    {
        throw new NotImplementedException();
    }

    public IItemStack? PickItems(int value)
    {
        throw new NotImplementedException();
    }

    public int RemoveItem(int number)
    {
        throw new NotImplementedException();
    }

    public void ClearStack()
    {
        throw new NotImplementedException();
    }
}