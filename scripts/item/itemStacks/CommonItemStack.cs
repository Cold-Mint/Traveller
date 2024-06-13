using System;

using Godot;

namespace ColdMint.scripts.item.itemStacks;

/// <summary>
/// <para>
///     one of the basic item stacks, where there is only one instance of an item actually held in the stack,
///     meaning that all items are identical (or completely random in some ways)
/// </para>
/// <para>平凡物品堆，基础物品堆之一，堆中实际保存的物品实例仅有一个，意味着所有物品都完全一致（或某些方面完全随机）</para>
/// </summary>
/// <param name="innerItem"></param>
/// <seealso cref="UniqueItemStack"/><seealso cref="SingleItemStack"/>
public class CommonItemStack(ICommonItem innerItem) : IItemStack
{
    public int MaxQuantity { get; } = ItemTypeManager.MaxStackQuantityOf(innerItem.Id);
    public int Quantity { get; private set; } = 1;
    public bool Empty => Quantity == 0;
    public Texture2D Icon => innerItem.Icon;
    public string Name => $"{innerItem.Name}({Quantity})";
    public string? Description => innerItem.Description;

    public bool CanAddItem(IItem item1)
    {
        return innerItem.CanStackWith(item1) && (Quantity < MaxQuantity);
    }

    public bool AddItem(IItem item)
    {
        if (!CanAddItem(item)) return false;
        Quantity++;
        return true;
    }

    public int CanTakeFrom(IItemStack itemStack)
    {
        if (itemStack.Empty || !innerItem.CanStackWith(itemStack.GetItem()!)) return 0;
        return Math.Min(itemStack.Quantity, MaxQuantity - Quantity);
    }

    public bool TakeFrom(IItemStack itemStack)
    {
        var number = CanTakeFrom(itemStack);
        itemStack.RemoveItem(number);
        Quantity += number;
        return itemStack.Empty;
    }

    public IItem? GetItem()
    {
        return Empty ? null : innerItem;
    }

    public IItem? PickItem()
    {
        if (Empty) return null;
        Quantity--;
        if (Empty) innerItem.Destroy();
        return innerItem.CopyInstance();
    }

    public IItemStack? PickItems(int value)
    {
        if (Empty) return null;
        var result = new CommonItemStack(innerItem.CopyInstance());
        var n = Math.Min(Quantity, value);
        result.Quantity = n;
        Quantity -= n;
        if (Empty) innerItem.Destroy();
        return result;
    }

    public int RemoveItem(int number)
    {
        var n = Math.Min(number, Quantity);
        Quantity -= n;
        if (Empty) innerItem.Destroy();
        return number - n;
    }

    public void ClearStack()
    {
        if (Empty) return;
        Quantity = 0;
        innerItem.Destroy();
    }
}