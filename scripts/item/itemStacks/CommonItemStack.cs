using System;
using Godot;

namespace ColdMint.scripts.item.itemStacks;

/// <summary>
/// <para>An ordinary item pile, in which only one instance of the item is actually saved.</para>
/// <para>普通的物品堆，堆中实际保存的物品实例仅有一个。</para>
/// </summary>
/// <remarks>
///<para>When the <see cref="AddItem"/> method is called in this implementation, the number of internal items is increased by one and new items passed in are destroyed.</para>
///<para>在此实现下调用<see cref="AddItem"/>方法时，会对内部物品的数量加一，并销毁传递进来的新物品。</para>
/// </remarks>
/// <param name="innerItem">
///<para>innerItem</para>
///<para>内部物品</para>
/// </param>
/// <seealso cref="UniqueItemStack"/>
/// <seealso cref="SingleItemStack"/>
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
        item.Destroy();
        return true;
    }

    public int CanTakeFrom(IItemStack itemStack)
    {
        var item = itemStack.GetItem();
        if (item == null)
        {
            return 0;
        }
        if (itemStack.Empty || !innerItem.CanStackWith(item)) return 0;
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
        if(Empty) return null;
        Quantity--;
        var result = innerItem.CloneInstance();
        if(Empty) innerItem.Destroy();
        return result;
    }

    public IItemStack? PickItems(int value)
    {
        if (Empty) return null;
        var result = new CommonItemStack(innerItem.CloneInstance());
        var n = Math.Min(Quantity, value);
        if (n < 0)
        {
            n = Quantity;
        }
        result.Quantity = n;
        Quantity -= n;
        if (Empty) innerItem.Destroy();
        return result;
    }

    public int RemoveItem(int number)
    {
        var n = Math.Min(number, Quantity);
        if (n < 0)
        {
            n = Quantity;
        }
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