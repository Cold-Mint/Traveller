using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ColdMint.scripts.map.events;
using ColdMint.scripts.utils;
using Godot;
using JetBrains.Annotations;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>UniversalItemContainer</para>
/// <para>通用的物品容器</para>
/// </summary>
public class UniversalItemContainer : IItemContainer
{
    private readonly List<ItemSlotNode>? _itemSlotNodes = [];

    private readonly PackedScene? _itemSlotPackedScene = GD.Load<PackedScene>("res://prefab/ui/ItemSlot.tscn");

    /// <summary>
    /// <para>UnknownIndex</para>
    /// <para>未知位置</para>
    /// </summary>
    private const int UnknownIndex = -1;

    //_selectIndex默认为0.
    private int _selectIndex;

    [MustDisposeResource]
    public IEnumerator<ItemSlotNode> GetEnumerator()
    {
        return _itemSlotNodes?.GetEnumerator() ?? Enumerable.Empty<ItemSlotNode>().GetEnumerator();
    }

    [MustDisposeResource]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Action<SelectedItemSlotChangeEvent>? SelectedItemSlotChangeEvent { get; set; }

    public bool CanAddItem(IItem item)
    {
        return Match(item) != null;
    }

    public bool AddItem(IItem item)
    {
        var itemSlotNode = Match(item);
        if (itemSlotNode == null)
        {
            return false;
        }

        return itemSlotNode.AddItem(item);
    }

    public bool SupportSelect { get; set; }

    public int GetSelectIndex()
    {
        return _selectIndex;
    }

    public ItemSlotNode? GetSelectItemSlotNode()
    {
        if (_itemSlotNodes == null || _itemSlotNodes.Count == 0)
        {
            return null;
        }

        if (_selectIndex < _itemSlotNodes.Count)
        {
            //Prevent subscripts from going out of bounds.
            //防止下标越界。
            return _itemSlotNodes[_selectIndex];
        }

        return null;
    }

    public int RemoveItemFromItemSlotBySelectIndex(int number) => RemoveItemFromItemSlot(_selectIndex, number);

    public int GetItemSlotCount()
    {
        if (_itemSlotNodes == null)
        {
            return 0;
        }

        return _itemSlotNodes.Count;
    }

    public ItemSlotNode? GetItemSlotNode(int index)
    {
        if (_itemSlotNodes == null)
        {
            return null;
        }

        var safeIndex = GetSafeIndex(index);
        return _itemSlotNodes[safeIndex];
    }

    public int RemoveItemFromItemSlot(int itemSlotIndex, int number)
    {
        if (_itemSlotNodes == null) return number;
        var safeIndex = GetSafeIndex(itemSlotIndex);
        if (safeIndex == UnknownIndex)
        {
            return number;
        }

        var itemSlot = _itemSlotNodes[safeIndex];
        return itemSlot.RemoveItem(number);
    }

    /// <summary>
    /// <para>Gets a secure subscript index</para>
    /// <para>获取安全的下标索引</para>
    /// </summary>
    /// <param name="itemSlotIndex"></param>
    /// <returns>
    /// <para>-1 is returned on failure, and the index that does not result in an out-of-bounds subscript is returned on success</para>
    /// <para>失败返回-1，成功返回不会导致下标越界的索引</para>
    /// </returns>
    private int GetSafeIndex(int itemSlotIndex)
    {
        if (_itemSlotNodes == null)
        {
            return UnknownIndex;
        }

        var count = _itemSlotNodes.Count;
        if (count == 0)
        {
            //Prevents the dividend from being 0
            //防止被除数为0
            return UnknownIndex;
        }

        return itemSlotIndex % count;
    }

    public ItemSlotNode? Match(IItem item)
    {
        //Find and return the first slot that can hold this item, if the list is null or not found, return null
        //寻找并返回第一个遇到的可放置此物品的物品槽，若列表为空或不存在，将返回null
        return _itemSlotNodes?.FirstOrDefault(itemSlotNode => itemSlotNode.CanAddItem(item));
    }

    public ItemSlotNode? AddItemSlot(Node rootNode)
    {
        if (_itemSlotNodes == null || _itemSlotPackedScene == null)
        {
            return null;
        }

        var itemSlotNode = NodeUtils.InstantiatePackedScene<ItemSlotNode>(_itemSlotPackedScene);
        if (itemSlotNode == null)
        {
            return null;
        }
        NodeUtils.CallDeferredAddChild(rootNode, itemSlotNode);
        if (SupportSelect)
        {
            itemSlotNode.IsSelect = _itemSlotNodes.Count == _selectIndex;
        }
        else
        {
            itemSlotNode.IsSelect = false;
        }

        _itemSlotNodes.Add(itemSlotNode);
        return itemSlotNode;
    }

    public void SelectTheNextItemSlot()
    {
        if (_itemSlotNodes == null)
        {
            return;
        }

        var count = _itemSlotNodes.Count;
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

        PrivateSelectItemSlot(oldSelectIndex, newSelectIndex);
    }

    public void SelectThePreviousItemSlot()
    {
        if (_itemSlotNodes == null)
        {
            return;
        }

        var count = _itemSlotNodes.Count;
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

        PrivateSelectItemSlot(oldSelectIndex, newSelectIndex);
    }

    /// <summary>
    /// <para>Select an item slot</para>
    /// <para>选中某个物品槽</para>
    /// </summary>
    private void PrivateSelectItemSlot(int oldSelectIndex, int newSelectIndex)
    {
        if (!SupportSelect || _itemSlotNodes == null || oldSelectIndex == newSelectIndex)
        {
            return;
        }

        var oldItemSlotNode = _itemSlotNodes[oldSelectIndex];
        oldItemSlotNode.IsSelect = false;
        var newItemSlotNode = _itemSlotNodes[newSelectIndex];
        newItemSlotNode.IsSelect = true;
        HideItem(oldSelectIndex);
        DisplayItem(newSelectIndex);
        SelectedItemSlotChangeEvent?.Invoke(new SelectedItemSlotChangeEvent
        {
            NewItemSlotNode = newItemSlotNode,
            OldItemSlotNode = oldItemSlotNode
        });
        _selectIndex = newSelectIndex;
    }

    /// <summary>
    /// <para>HideItem</para>
    /// <para>隐藏某个物品</para>
    /// </summary>
    /// <param name="index"></param>
    private void HideItem(int index)
    {
        var oldItem = _itemSlotNodes?[index].GetItem();
        if (oldItem is not Node2D oldNode2D) return;
        oldNode2D.ProcessMode = Node.ProcessModeEnum.Disabled;
        oldNode2D.Hide();
    }

    /// <summary>
    /// <para>Displays the items in an item slot</para>
    /// <para>显示某个物品槽内的物品</para>
    /// </summary>
    /// <remarks>
    ///<para>This method can also be used to refresh items held by the character, for example when a new item is dragged to the current display location, then call this method to refresh items held by the character.</para>
    ///<para>此方法也可用于刷新角色手上持有的物品，例如当新的物品被拖动到当前显示位置，那么请调用此方法刷新角色持有的物品。</para>
    /// </remarks>
    /// <param name="index"></param>
    private void DisplayItem(int index)
    {
        var item = _itemSlotNodes?[index].GetItem();
        if (item is not Node2D newNode2D) return;
        newNode2D.ProcessMode = Node.ProcessModeEnum.Inherit;
        newNode2D.Show();
    }

    public void SelectItemSlot(int newSelectIndex)
    {
        if (newSelectIndex == _selectIndex)
        {
            return;
        }

        var safeIndex = GetSafeIndex(newSelectIndex);
        if (safeIndex == UnknownIndex)
        {
            return;
        }

        PrivateSelectItemSlot(_selectIndex, newSelectIndex);
    }
}