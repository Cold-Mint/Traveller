using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using ColdMint.scripts.item;
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
    private readonly PackedScene? _itemSlotPackedScene = GD.Load<PackedScene>("res://prefab/ui/ItemSlot.tscn");

    private readonly List<ItemSlotNode>? _itemSlotNodes = new();

    /// <summary>
    /// <para>Character</para>
    /// <para>角色</para>
    /// </summary>
    public CharacterTemplate? CharacterTemplate { get; set; }

    /// <summary>
    /// <para>UnknownIndex</para>
    /// <para>未知位置</para>
    /// </summary>
    private const int UnknownIndex = -1;

    //_selectIndex默认为0.
    private int _selectIndex;

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

    public bool AddItemStack(IItemStack itemStack)
    {
        while (true)
        {
            var itemSlotNode = Match(itemStack);

            if (itemSlotNode == null)
                return false;

            if (itemSlotNode.AddItemStack(itemStack))
                return true;
        }
    }

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

    public IItem? PickItemFromItemSlotBySelectIndex() => PickItemFromItemSlot(_selectIndex);

    public IItemStack? PickItemsFromItemSlotBySelectIndex(int value) => PickItemsFromItemSlot(_selectIndex, value);

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

    public IItem? PickItemFromItemSlot(int itemSlotIndex)
    {
        if (_itemSlotNodes == null) return null;
        var safeIndex = GetSafeIndex(itemSlotIndex);
        if (safeIndex == UnknownIndex)
        {
            return null;
        }

        var itemSlot = _itemSlotNodes[safeIndex];
        return itemSlot.PickItem();
    }

    public IItemStack? PickItemsFromItemSlot(int itemSlotIndex, int value)
    {
        if (_itemSlotNodes == null) return null;
        var safeIndex = GetSafeIndex(itemSlotIndex);
        if (safeIndex == UnknownIndex)
        {
            return null;
        }

        var itemSlot = _itemSlotNodes[safeIndex];
        return itemSlot.PickItems(value);
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

    public ItemSlotNode? Match(IItem item)
    {
        //Find and return the first slot that can hold this item, if the list is null or not found, return null
        //寻找并返回第一个遇到的可放置此物品的物品槽，若列表为空或不存在，将返回null
        return _itemSlotNodes?.FirstOrDefault(itemSlotNode => itemSlotNode.CanAddItem(item));
    }

    public ItemSlotNode? Match(IItemStack stack)
    {
        return _itemSlotNodes?.FirstOrDefault(itemSlotNode => itemSlotNode.CanAddItem(stack.GetItem()!));
    }

    public ItemSlotNode? Match(Func<IItemStack?, bool> predicate)
    {
        return _itemSlotNodes?.FirstOrDefault(node => predicate(node.GetItemStack()));
    }

    public IEnumerable<ItemSlotNode> MatchAll(Func<IItemStack?, bool> predicate)
    {
        return from node in _itemSlotNodes where predicate(node.GetItemStack()) select node;
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

    /// <summary>
    /// <para>Add items tank</para>
    /// <para>添加物品槽</para>
    /// </summary>
    public void AddItemSlot(Node rootNode, int index)
    {
        if (_itemSlotNodes == null || _itemSlotPackedScene == null)
        {
            return;
        }

        var itemSlotNode = NodeUtils.InstantiatePackedScene<ItemSlotNode>(_itemSlotPackedScene, rootNode);
        if (itemSlotNode == null)
        {
            return;
        }

        itemSlotNode.IsSelect = index == _selectIndex;
        _itemSlotNodes.Add(itemSlotNode);
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

    /// <summary>
    /// <para>Select an item slot</para>
    /// <para>选中某个物品槽</para>
    /// </summary>
    private void PrivateSelectItemSlot(int oldSelectIndex, int newSelectIndex)
    {
        if (oldSelectIndex == newSelectIndex)
        {
            return;
        }

        if (_itemSlotNodes == null)
        {
            return;
        }

        _itemSlotNodes[oldSelectIndex].IsSelect = false;
        _itemSlotNodes[newSelectIndex].IsSelect = true;
        var oldItem = _itemSlotNodes[oldSelectIndex].GetItem();
        if (oldItem is Node2D oldNode2D)
        {
            oldNode2D.ProcessMode = Node.ProcessModeEnum.Disabled;
            oldNode2D.Hide();
        }

        var item = _itemSlotNodes[newSelectIndex].GetItem();
        switch (item)
        {
            case null:
            {
                if (CharacterTemplate != null)
                {
                    CharacterTemplate.CurrentItem = null;
                }

                break;
            }
            case Node2D node2D:
            {
                node2D.ProcessMode = Node.ProcessModeEnum.Inherit;
                node2D.Show();
                if (CharacterTemplate != null)
                {
                    CharacterTemplate.CurrentItem = node2D;
                }

                break;
            }
            default:
            {
                if (CharacterTemplate != null)
                {
                    CharacterTemplate.CurrentItem = null;
                }

                break;
            }
        }

        _selectIndex = newSelectIndex;
    }


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
}