using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColdMint.scripts.debug;
using ColdMint.scripts.map.events;

namespace ColdMint.scripts.inventory;

public abstract class ItemContainerDisplayTemplate : IItemContainerDisplay
{
    protected readonly List<IItemDisplay> ItemDisplayList = [];
    private IItemContainer? _itemContainer;

    public async void BindItemContainer(IItemContainer? itemContainer)
    {
        if (_itemContainer == itemContainer)
        {
            return;
        }

        if (_itemContainer != null)
        {
            _itemContainer.SelectedItemChangeEvent -= OnSelectedItemChangeEvent;
            _itemContainer.ItemDataChangeEvent -= OnItemDataChangeEvent;
        }

        _itemContainer = itemContainer;
        if (itemContainer == null)
        {
            //Set empty items container to hide all ui.
            //设置空物品容器，隐藏全部ui。
            for (var i = 0; i < ItemDisplayList.Count; i++)
            {
                var itemDisplay = ItemDisplayList[i];
                LogCat.LogWithFormat("hide_display_item", LogCat.LogLabel.ItemContainerDisplay, i);
                itemDisplay.Update(null);
                itemDisplay.HideSelf();
            }
            return;
        }
        itemContainer.SelectedItemChangeEvent += OnSelectedItemChangeEvent;
        itemContainer.ItemDataChangeEvent += OnItemDataChangeEvent;
        var totalCapacity = itemContainer.GetTotalCapacity();
        var currentCapacity = ItemDisplayList.Count;
        var capacityDifference = totalCapacity - currentCapacity;
        if (capacityDifference > 0)
        {
            //There are those that need to be added, and we create them.
            //有需要添加的，我们创建他们。
            for (var i = 0; i < capacityDifference; i++)
            {
                LogCat.LogWithFormat("add_display_item", LogCat.LogLabel.ItemContainerDisplay, i);
                AddItemDisplay();
            }
        }
        else if (capacityDifference < 0)
        {
            //There are things that need to be hidden
            //有需要被隐藏的
            for (var i = currentCapacity - 1; i >= totalCapacity; i--)
            {
                LogCat.LogWithFormat("hide_display_item", LogCat.LogLabel.ItemContainerDisplay, i);
                var itemDisplay = ItemDisplayList[i];
                itemDisplay.Update(null);
                itemDisplay.HideSelf();
            }
        }

        await Task.Yield();
        UpdateData(itemContainer, totalCapacity);
    }

    private void OnItemDataChangeEvent(ItemDataChangeEvent itemDataChangeEvent)
    {
        if (_itemContainer == null)
        {
            return;
        }

        UpdateDataForSingleLocation(_itemContainer, itemDataChangeEvent.NewIndex);
    }

    private void OnSelectedItemChangeEvent(SelectedItemChangeEvent selectedItemChangeEvent)
    {
        if (_itemContainer == null)
        {
            return;
        }

        UpdateDataForSingleLocation(_itemContainer, selectedItemChangeEvent.OldIndex);
        UpdateDataForSingleLocation(_itemContainer, selectedItemChangeEvent.NewIndex);
    }

    /// <summary>
    /// <para>Update data</para>
    /// <para>更新数据</para>
    /// </summary>
    /// <remarks>
    ///<para>Used to batch update the Item data in itemContainer to the display.</para>
    ///<para>用于将itemContainer内的Item数据批量更新到显示器内。</para>
    /// </remarks>
    /// <param name="itemContainer">
    ///<para>Item container data</para>
    ///<para>物品容器数据</para>
    /// </param>
    /// <param name="endIndex">
    ///<para>endIndex</para>
    ///<para>结束位置</para>
    /// </param>
    /// <param name="startIndex">
    ///<para>startIndex</para>
    ///<para>起始位置</para>
    /// </param>
    private void UpdateData(IItemContainer itemContainer, int endIndex, int startIndex = 0)
    {
        LogCat.LogWithFormat("batch_update_data", LogCat.LogLabel.ItemContainerDisplay, startIndex, endIndex);
        for (var i = startIndex; i < endIndex; i++)
        {
            UpdateDataForSingleLocation(itemContainer, i);
        }
    }

    /// <summary>
    /// <para>Update data for a single location</para>
    /// <para>更新单个位置的数据</para>
    /// </summary>
    /// <param name="itemContainer"></param>
    /// <param name="index"></param>
    private void UpdateDataForSingleLocation(IItemContainer itemContainer, int index)
    {
        LogCat.LogWithFormat("update_display_item", LogCat.LogLabel.ItemContainerDisplay, index);
        var itemDisplay = ItemDisplayList[index];
        var item = itemContainer.GetItem(index);
        if (item == null)
        {
            item = itemContainer.GetPlaceHolderItem(index);
        }
        if (itemContainer.SupportSelect)
        {
            item.IsSelect = index == itemContainer.GetSelectIndex();
        }
        else
        {
            item.IsSelect = false;
        }
        itemDisplay.Update(item);
        itemDisplay.ShowSelf();
    }

    /// <summary>
    /// <para>Add item display</para>
    /// <para>添加物品显示器</para>
    /// </summary>
    protected abstract void AddItemDisplay();

    public IEnumerator<IItemDisplay> GetEnumerator()
    {
        return ItemDisplayList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}