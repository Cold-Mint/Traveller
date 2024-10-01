using System;
using System.Collections.Generic;
using ColdMint.scripts.map.events;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>UniversalItemContainer</para>
/// <para>通用的物品容器</para>
/// </summary>
public class UniversalItemContainer(int totalCapacity) : IItemContainer
{
    private readonly Dictionary<int, IItem> _itemDictionary = [];

    /// <summary>
    /// <para>UnknownIndex</para>
    /// <para>未知位置</para>
    /// </summary>
    private const int UnknownIndex = -1;

    //_selectIndex defaults to 0.
    //_selectIndex默认为0.
    private int _selectIndex;

    /// <summary>
    /// <para>The next available index</para>
    /// <para>下个可用的索引</para>
    /// </summary>
    /// <remarks>
    ///<para>For example, the variable [1,2,3,5,6] represents 4, or the variable [1,2,3,4,5,6,7] represents 8.</para>
    ///<para>例如[1,2,3,5,6]这个变量表示4，再或者[1,2,3,4,5,6,7]这个变量表示8。</para>
    /// </remarks>
    private int _nextAvailableIndex;

    public Action<SelectedItemChangeEvent>? SelectedItemChangeEvent { get; set; }
    public Action<ItemDataChangeEvent>? ItemDataChangeEvent { get; set; }

    public bool CanAddItem(IItem item)
    {
        if (item.SelfItemContainer != null && !CanContainContainer)
        {
            //The item to be added can hold other items, and this item container does not allow item containers.
            //要添加的物品能够容纳其他物品，且此物品容器不允许放置物品容器。
            return false;
        }
        //If the capacity is not full, directly return to add items
        //如果未占满容量，直接返回可添加物品
        if (GetUsedCapacity() < totalCapacity)
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
        foreach (var unitItem in _itemDictionary.Values)
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

    private void UpdateSelectStatus(int index, IItem item)
    {
        item.IsSelect = index == _selectIndex;
    }

    /// <summary>
    /// <para>Update the next available index location</para>
    /// <para>更新下个可用的索引位置</para>
    /// </summary>
    private void UpdateNextAvailableIndex()
    {
        _nextAvailableIndex = UnknownIndex;
        if (totalCapacity <= 0)
        {
            return;
        }
        for (var i = 0; i < totalCapacity; i++)
        {
            var contains = _itemDictionary.ContainsKey(i);
            if (!contains)
            {
                _nextAvailableIndex = i;
                return;
            }
        }
    }

    public int AddItem(IItem item)
    {
        if (item.MaxQuantity == 1)
        {
            if (_nextAvailableIndex == UnknownIndex)
            {
                return 0;
            }

            var nextAvailableIndex = _nextAvailableIndex;
            _itemDictionary[nextAvailableIndex] = item;
            item.Index = nextAvailableIndex;
            item.ItemContainer = this;
            if (nextAvailableIndex != _selectIndex)
            {
                item.HideSelf();
            }
            UpdateNextAvailableIndex();
            UpdateSelectStatus(nextAvailableIndex, item);
            ItemDataChangeEvent?.Invoke(new ItemDataChangeEvent
            {
                NewItem = item,
                NewIndex = nextAvailableIndex,
                Type = Config.ItemDataChangeEventType.QuantityAdded
            });
            return item.Quantity;
        }

        //There can be more than one item, try to share equally.
        //物品可有多个，尝试均摊。
        var originalQuantity = item.Quantity;
        var temporarilyQuantity = item.Quantity;
        var index = 0;
        foreach (var unitItem in _itemDictionary.Values)
        {
            var number = unitItem.MergeableItemCount(item, temporarilyQuantity);
            if (number == 0)
            {
                continue;
            }

            temporarilyQuantity -= number;
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
                item.HideSelf();
                return originalQuantity;
            }

            index++;
        }

        //New items have some left over.
        //新物品有一些剩余。
        if (GetUsedCapacity() >= totalCapacity)
        {
            //The capacity is full. The remaining capacity cannot be stored.
            //容量已满，无法存放剩余。
            return originalQuantity - temporarilyQuantity;
        }

        //Add the rest to the container.
        //添加剩余到容器内。
        if (_nextAvailableIndex == UnknownIndex)
        {
            return 0;
        }
        var finalNextAvailableIndex = _nextAvailableIndex;
        _itemDictionary[finalNextAvailableIndex] = item;
        item.Index = finalNextAvailableIndex;
        item.ItemContainer = this;
        if (finalNextAvailableIndex != _selectIndex)
        {
            item.HideSelf();
        }
        UpdateNextAvailableIndex();
        UpdateSelectStatus(finalNextAvailableIndex, item);
        ItemDataChangeEvent?.Invoke(new ItemDataChangeEvent
        {
            NewItem = item,
            NewIndex = finalNextAvailableIndex,
            Type = Config.ItemDataChangeEventType.Add
        });
        return originalQuantity;
    }

    public bool SupportSelect { get; set; }
    public bool CanContainContainer { get; set; }


    public IItem GetPlaceHolderItem(int index)
    {
        var placeholderItem = new PlaceholderItem
        {
            Index = index,
            ItemContainer = this
        };
        return placeholderItem;
    }

    public int GetSelectIndex()
    {
        return _selectIndex;
    }

    public IItem? GetSelectItem()
    {
        return _itemDictionary.TryGetValue(_selectIndex, out var item) ? item : null;
    }

    public IItem? GetItem(int index)
    {
        return _itemDictionary.TryGetValue(index, out var item) ? item : null;
    }

    public bool ReplaceItem(int index, IItem item)
    {
        var oldItem = GetItem(index);
        _itemDictionary[index] = item;
        item.Index = index;
        item.ItemContainer = this;
        ItemDataChangeEvent?.Invoke(new ItemDataChangeEvent
        {
            NewItem = item,
            NewIndex = index,
            OldIndex = index,
            OldItem = oldItem,
            Type = Config.ItemDataChangeEventType.Replace
        });
        return true;
    }

    public bool CanReplaceItem(int index, IItem item)
    {
        if (item.SelfItemContainer != null && !CanContainContainer)
        {
            return false;
        }
        return true;
    }


    public bool ClearItem(int index)
    {
        if (!_itemDictionary.TryGetValue(index, out var item))
        {
            return false;
        }
        var result = _itemDictionary.Remove(index);
        if (result)
        {
            ItemDataChangeEvent?.Invoke(new ItemDataChangeEvent
            {
                NewItem = null,
                NewIndex = index,
                OldIndex = index,
                OldItem = null,
                Type = Config.ItemDataChangeEventType.Clear
            });
            if (SupportSelect && index == _selectIndex)
            {
                item.HideSelf();
                SelectedItemChangeEvent?.Invoke(new SelectedItemChangeEvent()
                {
                    NewIndex = index,
                    OldIndex = index,
                    NewItem = null,
                    OldItem = null
                });
            }
        }
        return result;
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

        if (!_itemDictionary.TryGetValue(itemIndex, out var item))
        {
            return 0;
        }

        var originalQuantity = item.Quantity;
        if (number < 0)
        {
            //If the number entered is less than 0, all items are removed.
            //输入的数量小于0,则移除全部物品。
            item.Quantity = 0;
            _itemDictionary.Remove(itemIndex);
            UpdateNextAvailableIndex();
            ItemDataChangeEvent?.Invoke(new ItemDataChangeEvent
            {
                NewItem = item,
                NewIndex = itemIndex,
                Type = Config.ItemDataChangeEventType.Remove
            });
            return originalQuantity;
        }

        var removed = Math.Min(number, item.Quantity);
        item.Quantity -= removed;
        if (item.Quantity < 1)
        {
            _itemDictionary.Remove(itemIndex);
            UpdateNextAvailableIndex();
            ItemDataChangeEvent?.Invoke(new ItemDataChangeEvent
            {
                NewItem = item,
                NewIndex = itemIndex,
                Type = Config.ItemDataChangeEventType.Remove
            });
        }

        return removed;
    }

    public int GetUsedCapacity()
    {
        return _itemDictionary.Count;
    }

    public int GetTotalCapacity()
    {
        return totalCapacity;
    }


    public void SelectNextItem()
    {
        var count = totalCapacity;
        if (count == 0)
        {
            return;
        }

        var oldSelectIndex = _selectIndex;
        var newSelectIndex = _selectIndex + 1;
        if (newSelectIndex >= count)
        {
            newSelectIndex = 0;
        }

        PrivateSelectItem(oldSelectIndex, newSelectIndex);
    }

    public void SelectPreviousItem()
    {
        var count = totalCapacity;
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

        //There is no need to broadcast placeholders when an event is invoked.
        //在调用事件时，无需广播占位符。
        var oldItem = GetItem(oldIndex);
        if (oldItem != null)
        {
            oldItem.HideSelf();
            oldItem.IsSelect = false;
        }

        //There is no need to broadcast placeholders when an event is invoked.
        //在调用事件时，无需广播占位符。
        var newItem = GetItem(newIndex);
        if (newItem != null)
        {
            newItem.ShowSelf();
            newItem.IsSelect = true;
        }

        _selectIndex = newIndex;
        SelectedItemChangeEvent?.Invoke(new SelectedItemChangeEvent
        {
            NewIndex = newIndex,
            OldIndex = oldIndex,
            NewItem = newItem,
            OldItem = oldItem
        });
    }


    public void SelectItem(int index)
    {
        if (totalCapacity == 0 || index < 0)
        {
            return;
        }
        PrivateSelectItem(_selectIndex, index % totalCapacity);
    }
}