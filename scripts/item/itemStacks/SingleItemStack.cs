using System;

using Godot;

namespace ColdMint.scripts.item.itemStacks;

/// <summary>
/// <para>One of the basic item stacks, there are always one item in stack(Stack not supported)</para>
/// <para>单身狗物品堆，基础物品堆之一，堆中永远只会有一个物品（不支持堆叠）</para>
/// </summary>
/// <seealso cref="UniqueItemStack"/>
/// <seealso cref="CommonItemStack"/>
public class SingleItemStack(IItem item) : IItemStack
{
    public IItem Item { get; init; } = item;

    public int MaxQuantity => 1;
    public int Quantity => 1;
    public bool Empty { get; private set; }
    public Texture2D Icon => Item.Icon;
    public string Name => Item.Name;
    public string? Description => Item.Description;

    public bool CanAddItem(IItem item) => false;

    public bool AddItem(IItem item) => false;

    public int CanTakeFrom(IItemStack itemStack) => 0;

    public bool TakeFrom(IItemStack itemStack) => false;

    public IItem? GetItem()
    {
        return Empty ? null : Item;
    }

    public IItem? PickItem()
    {
        if (Empty) return null;
        Empty = true;
        return Item;
    }

    public IItemStack? PickItems(int value)
    {
        if (value == 0 || Empty) return null;

        Empty = true;
        return new SingleItemStack(Item);
    }

    public int RemoveItem(int number)
    {
        if (number == 0 || Empty) return 0;
        Empty = true;
        Item.Destroy();
        return Math.Max(number - 1, 0);
    }

    public void ClearStack()
    {
        RemoveItem(1);
    }
}