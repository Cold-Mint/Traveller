using System;

using Godot;

namespace ColdMint.scripts.item.itemStacks;

/// <summary>
/// <para>ItemStack for <see cref="Packsack"/> item. Can add items into pack by stack them to this stack.</para>
/// <para>用于<see cref="Packsack"/>物品的堆栈。可以将物品堆叠到此堆栈中，从而将物品添加到背包中。</para>
/// </summary>
/// <param name="packsack"></param>
public class PacksackStack(Packsack packsack) : IItemStack
{
    public int MaxQuantity => 1;
    public int Quantity => 1;
    public bool Empty { get; private set; }
    public Texture2D Icon => packsack.Icon;
    public string Name => packsack.Name;
    public string? Description => packsack.Description;

    //todo: 只拒绝是背包的物品是权宜之计，应该为物品加入一个“是否可以放入背包”的属性来实现这个判断。
    public bool CanAddItem(IItem item)
    {
        if (item is Packsack) return false;
        return packsack.ItemContainer?.CanAddItem(item) ?? false;
    }

    public bool AddItem(IItem item)
    {
        if (item is Packsack) return false;
        return packsack.ItemContainer?.AddItem(item) ?? false;
    }

    public int CanTakeFrom(IItemStack itemStack)
    {
        if (itemStack.GetItem() is Packsack) return 0;
        return packsack.ItemContainer?.CanAddItemStack(itemStack) ?? 0;
    }

    public bool TakeFrom(IItemStack itemStack)
    {
        if (itemStack.GetItem() is Packsack) return false;
        return packsack.ItemContainer?.AddItemStack(itemStack) ?? false;
    }

    public IItem? GetItem()
    {
        return Empty ? packsack : null;
    }

    public IItem? PickItem()
    {
        if (Empty) return null;
        Empty = true;
        return packsack;
    }

    public IItemStack? PickItems(int value)
    {
        if (Empty || value == 0) return null;
        Empty = true;
        return new PacksackStack(packsack);
    }

    public int RemoveItem(int number)
    {
        if (Empty || number == 0) return number;
        Empty = true;
        packsack.Destroy();
        return Math.Max(0, number - 1);
    }

    public void ClearStack()
    {
        if (Empty) return;
        packsack.Destroy();
        Empty = true;
    }
}