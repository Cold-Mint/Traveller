using System;
using System.Collections;
using System.Collections.Generic;
using ColdMint.scripts.map.events;
using JetBrains.Annotations;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>UniversalItemContainer</para>
/// <para>通用的物品容器</para>
/// </summary>
public class UniversalItemContainer(int totalCapacity) : IItemContainer
{
    private readonly List<IItem> _itemList = [];

    /// <summary>
    /// <para>UnknownIndex</para>
    /// <para>未知位置</para>
    /// </summary>
    private const int UnknownIndex = -1;

    //_selectIndex defaults to 0.
    //_selectIndex默认为0.
    private int _selectIndex;

    [MustDisposeResource]
    public IEnumerator<IItem> GetEnumerator()
    {
        return _itemList.GetEnumerator();
    }

    [MustDisposeResource]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Action<SelectedItemChangeEvent>? SelectedItemChangeEvent { get; set; }
    public Action<ItemDataChangeEvent>? ItemDataChangeEvent { get; set; }

    public bool CanAddItem(IItem item)
    {
        //If the capacity is not full, directly return to add items
        //如果未占满容量，直接返回可添加物品
        if (GetUsedCapacity() < GetTotalCapacity())
        {
            return true;
        }

        if (item.MaxQuantity == 1)
        {
            //New items do not support overlay, capacity is full, return cannot add.
            //新物品不支持叠加，容量已满，返回不能添加。
            return false;
        }

        //If the capacity is full, we calculate whether we can spread the new items evenly among the existing items.
        //如果容量占满了，我们计算是否能将新物品均摊在已有的物品内。
        var unallocatedQuantity = item.Quantity;
        foreach (var unitItem in _itemList)
        {
            var number = unitItem.MergeableItemCount(item, unallocatedQuantity);
            if (number == 0)
            {
                continue;
            }

            unallocatedQuantity -= number;
            if (unallocatedQuantity < 1)
            {
                return true;
            }
        }

        return unallocatedQuantity < 1;
    }


    public int AddItem(IItem item)
    {
        if (item.MaxQuantity == 1)
        {
            if (GetUsedCapacity() >= GetTotalCapacity())
            {
                //Items cannot be stacked and cannot be added if the capacity is full.
                //物品不能叠加，且容量已满，则无法添加。
                return 0;
            }

            _itemList.Add(item);
            ItemDataChangeEvent?.Invoke(new ItemDataChangeEvent
            {
                NewItem = item,
                NewIndex = _itemList.Count - 1,
                Type = Config.ItemDataChangeEventType.QuantityAdded
            });
            return item.Quantity;
        }

        //There can be more than one item, try to share equally.
        //物品可有多个，尝试均摊。
        var originalQuantity = item.Quantity;
        var index = 0;
        foreach (var unitItem in _itemList)
        {
            var number = unitItem.MergeableItemCount(item, item.Quantity);
            if (number == 0)
            {
                continue;
            }

            item.Quantity -= number;
            unitItem.Quantity += number;
            ItemDataChangeEvent?.Invoke(new ItemDataChangeEvent
            {
                NewItem = unitItem,
                NewIndex = index,
                Type = Config.ItemDataChangeEventType.QuantityAdded
            });
            if (item.Quantity < 1)
            {
                //New items are fully shared.
                //新物品完全被均摊。
                return originalQuantity;
            }

            index++;
        }

        //New items have some left over.
        //新物品有一些剩余。
        if (GetUsedCapacity() >= GetTotalCapacity())
        {
            //The capacity is full. The remaining capacity cannot be stored.
            //容量已满，无法存放剩余。
            return originalQuantity - item.Quantity;
        }

        //Add the rest to the container.
        //添加剩余到容器内。
        _itemList.Add(item);
        ItemDataChangeEvent?.Invoke(new ItemDataChangeEvent
        {
            NewItem = item,
            NewIndex = _itemList.Count - 1,
            Type = Config.ItemDataChangeEventType.Add
        });
        return originalQuantity;
    }

    public bool SupportSelect { get; set; }

    public int GetSelectIndex()
    {
        return _selectIndex;
    }

    public IItem? GetSelectItem()
    {
        return _itemList.Count == 0 ? null : _itemList[_selectIndex];
    }

    public IItem? GetItem(int index)
    {
        var safeIndex = GetSafeIndex(index);
        if (safeIndex == UnknownIndex)
        {
            return null;
        }

        return _itemList[safeIndex];
    }

    /// <summary>
    /// <para>Gets a secure subscript index</para>
    /// <para>获取安全的下标索引</para>
    /// </summary>
    /// <param name="index"></param>
    /// <returns>
    /// <para>-1 is returned on failure, and the index that does not result in an out-of-bounds subscript is returned on success</para>
    /// <para>失败返回-1，成功返回不会导致下标越界的索引</para>
    /// </returns>
    private int GetSafeIndex(int index)
    {
        var count = _itemList.Count;
        if (count == 0)
        {
            //Prevents the dividend from being 0
            //防止被除数为0
            return UnknownIndex;
        }

        return index % count;
    }


    public int RemoveSelectItem(int number)
    {
        return RemoveItem(_selectIndex, number);
    }

    public int RemoveItem(int itemIndex, int number)
    {
        if (number == 0)
        {
            return 0;
        }

        var safeIndex = GetSafeIndex(itemIndex);
        if (safeIndex == UnknownIndex)
        {
            return 0;
        }

        var item = _itemList[safeIndex];
        var originalQuantity = item.Quantity;
        if (number < 0)
        {
            //If the number entered is less than 0, all items are removed.
            //输入的数量小于0,则移除全部物品。
            item.Quantity = 0;
            _itemList.RemoveAt(safeIndex);
            return originalQuantity;
        }

        var removed = Math.Min(number, item.Quantity);
        item.Quantity -= removed;
        if (item.Quantity < 1)
        {
            _itemList.RemoveAt(safeIndex);
        }

        return removed;
    }

    public int GetUsedCapacity()
    {
        return _itemList.Count;
    }

    public int GetTotalCapacity()
    {
        return totalCapacity;
    }


    public void SelectNextItem()
    {
        var count = _itemList.Count;
        if (count == 0)
        {
            return;
        }

        var oldSelectIndex = _selectIndex;
        var newSelectIndex = _selectIndex + 1;
        if (newSelectIndex >= count)
        {
            newSelectIndex = count - 1;
        }

        PrivateSelectItem(oldSelectIndex, newSelectIndex);
    }

    public void SelectPreviousItem()
    {
        var count = _itemList.Count;
        if (count == 0)
        {
            return;
        }

        var oldSelectIndex = _selectIndex;
        var newSelectIndex = _selectIndex - 1;
        if (newSelectIndex < 0)
        {
            newSelectIndex = count - 1;
        }

        PrivateSelectItem(oldSelectIndex, newSelectIndex);
    }

    /// <summary>
    /// <para>Private methods for selecting items</para>
    /// <para>选择物品的私有方法</para>
    /// </summary>
    /// <param name="oldIndex"></param>
    /// <param name="newIndex"></param>
    private void PrivateSelectItem(int oldIndex, int newIndex)
    {
        if (!SupportSelect || oldIndex == newIndex)
        {
            return;
        }

        var oldItem = _itemList[oldIndex];
        oldItem.IsSelect = false;
        var newItem = _itemList[newIndex];
        newItem.IsSelect = true;
        SelectedItemChangeEvent?.Invoke(new SelectedItemChangeEvent
        {
            NewIndex = newIndex,
            OldIndex = oldIndex,
            NewItem = newItem,
            OldItem = oldItem
        });
        _selectIndex = newIndex;
    }


    public void SelectItem(int index)
    {
        var safeIndex = GetSafeIndex(index);
        if (safeIndex == UnknownIndex)
        {
            return;
        }

        PrivateSelectItem(_selectIndex, safeIndex);
    }
}