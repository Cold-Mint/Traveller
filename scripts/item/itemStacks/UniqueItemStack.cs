using System;
using System.Collections.Generic;
using Godot;

namespace ColdMint.scripts.item.itemStacks;

/// <summary>
/// <para>One of the basic item stacks, where each item in the stack maintains its original state (allowing for items that are not identical to each other)</para>
/// <para>独特物品堆，基础物品堆之一，堆中的每一个物品会保持各自原本的状态（允许互不相同的物品存在）</para> 
/// </summary>
/// <seealso cref="CommonItemStack"/><seealso cref="SingleItemStack"/>
public class UniqueItemStack : IItemStack
{
    private readonly Stack<IItem> _items;

    public UniqueItemStack(IItem item)
    {
        _items = [];
        _items.Push(item);
        MaxQuantity = ItemTypeManager.MaxStackQuantityOf(item.Id);
    }

    
    private UniqueItemStack(UniqueItemStack from)
    {
        _items = from._items;
        MaxQuantity = from.MaxQuantity;
        Quantity = from.Quantity;
        from.Quantity = 0;
    }

    public int MaxQuantity { get; }
    public int Quantity { get; private set; } = 1;
    public bool Empty => Quantity == 0;
    public Texture2D Icon => GetItem()?.Icon ?? new PlaceholderTexture2D();
    public string Name => $"{GetItem()?.Name}({Quantity})";
    public string? Description => GetItem()?.Description;

    public bool CanAddItem(IItem item)
    {
        //当两个物品相容，且当前堆未满时，我们返回true
        return (GetItem()?.CanStackWith(item) ?? false) && (Quantity < MaxQuantity);
    }

    public bool AddItem(IItem item)
    {
        if (!CanAddItem(item)) return false;
        _items.Push(item);
        Quantity++;
        return true;
    }

    public int CanTakeFrom(IItemStack itemStack)
    {
        if (!(itemStack.GetItem() is { } item)) return 0;
        //如果两个物品相容，那么可以获取的数量取决于当前物品堆空位的大小和对方物品数量中较小的一方
        if (CanAddItem(item))
            return Mathf.Min(itemStack.Quantity, MaxQuantity - Quantity);
        else return 0;
    }

    public bool TakeFrom(IItemStack itemStack)
    {
        var value = CanTakeFrom(itemStack);
        for (int i = 0; i < value; i++)
        {
            //一个如果代码没有出错就用不到的安全检查
            if (!AddItem(itemStack.PickItem()!)) break;
        }

        return itemStack.Empty;
    }

    public IItem? GetItem()
    {
        return Empty ? null : _items.Peek();
    }

    public IItem? PickItem()
    {
        if (Empty) return null;
        Quantity--;
        return _items.Pop();
    }

    public IItemStack? PickItems(int value)
    {
        if (Empty) return null;

        if (value < 0) value = Quantity;

        var result = new UniqueItemStack(PickItem()!);
        //Calculate the amount left to take out
        //计算剩余的要取出的数量
        var restToMove = Math.Min(value - 1, Quantity);
        for (var i = 0; i < restToMove; i++)
        {
            result.AddItem(PickItem()!);
        }

        return result;
    }

    public int RemoveItem(int number)
    {
        if (number < 0) number = Quantity;
        while (number > 0 && Quantity > 0)
        {
            PickItem()!.Destroy();
            number--;
        }

        return number;
    }

    public void ClearStack()
    {
        while (Quantity > 0)
        {
            PickItem()!.Destroy();
        }
    }
}