using System.Collections.Generic;
using System.Linq;
using ColdMint.scripts.debug;
using ColdMint.scripts.weapon;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>HotBar</para>
/// <para>快捷物品栏</para>
/// </summary>
public partial class HotBar : HBoxContainer
{
    private PackedScene _itemSlotPackedScene;
    private List<ItemSlotNode> _itemSlotNodes;
    private int selectIndex = 0;

    public override void _Ready()
    {
        base._Ready();
        _itemSlotNodes = new List<ItemSlotNode>();
        _itemSlotPackedScene = GD.Load<PackedScene>("res://prefab/ui/ItemSlot.tscn");
        for (var i = 0; i < Config.HotBarSize; i++)
        {
            AddItemSlot(i);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionPressed("throw"))
        {
            //Players are not allowed to switch current items while throwing them.
            //玩家在抛物品时禁止切换当前物品。
            return;
        }

        if (Input.IsActionJustPressed("hotbar_next"))
        {
            var count = _itemSlotNodes.Count;
            if (count == 0)
            {
                return;
            }

            //Mouse wheel down
            //鼠标滚轮向下
            var oldSelectIndex = selectIndex;
            selectIndex++;
            if (selectIndex >= count)
            {
                selectIndex = 0;
            }

            SelectItemSlot(oldSelectIndex, selectIndex);
        }

        if (Input.IsActionJustPressed("hotbar_previous"))
        {
            var count = _itemSlotNodes.Count;
            if (count == 0)
            {
                return;
            }

            //Mouse wheel up
            //鼠标滚轮向上
            var oldSelectIndex = selectIndex;
            selectIndex--;
            if (selectIndex < 0)
            {
                selectIndex = count - 1;
            }

            SelectItemSlot(oldSelectIndex, selectIndex);
        }

        if (Input.IsActionJustPressed("hotbar_1"))
        {
            SelectItemSlotByHotBarShortcutKey(0);
        }

        if (Input.IsActionJustPressed("hotbar_2"))
        {
            SelectItemSlotByHotBarShortcutKey(1);
        }

        if (Input.IsActionJustPressed("hotbar_3"))
        {
            SelectItemSlotByHotBarShortcutKey(2);
        }

        if (Input.IsActionJustPressed("hotbar_4"))
        {
            SelectItemSlotByHotBarShortcutKey(3);
        }

        if (Input.IsActionJustPressed("hotbar_5"))
        {
            SelectItemSlotByHotBarShortcutKey(4);
        }

        if (Input.IsActionJustPressed("hotbar_6"))
        {
            SelectItemSlotByHotBarShortcutKey(5);
        }

        if (Input.IsActionJustPressed("hotbar_7"))
        {
            SelectItemSlotByHotBarShortcutKey(6);
        }

        if (Input.IsActionJustPressed("hotbar_8"))
        {
            SelectItemSlotByHotBarShortcutKey(7);
        }

        if (Input.IsActionJustPressed("hotbar_9"))
        {
            SelectItemSlotByHotBarShortcutKey(8);
        }
    }

    /// <summary>
    /// <para>Select the HotBar project using the shortcut keys</para>
    /// <para>通过快捷键选择HotBar项目</para>
    /// </summary>
    /// <para>The Pc version of the shortcut key index is 0-9</para>
    /// <para>Pc版本的快捷键索引为0-9</para>
    /// <param name="shortcutKeyIndex"></param>
    private void SelectItemSlotByHotBarShortcutKey(int shortcutKeyIndex)
    {
        var count = _itemSlotNodes.Count;
        if (count == 0)
        {
            //Prevents the dividend from being 0
            //防止被除数为0
            return;
        }

        var newIndex = shortcutKeyIndex % count;
        SelectItemSlot(selectIndex, newIndex);
        selectIndex = newIndex;
    }


    /// <summary>
    /// <para>Removes an item from the currently selected inventory</para>
    /// <para>移除当前选中的物品栏内的物品</para>
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public bool RemoveItemFromItemSlotBySelectIndex(int number)
    {
        return RemoveItemFromItemSlot(selectIndex, number);
    }

    /// <summary>
    /// <para>Remove items from the item slot</para>
    /// <para>从物品槽内移除物品</para>
    /// </summary>
    /// <param name="itemSlotIndex">
    ///<para>When this number is greater than the number of item slots, residual filtering is used.</para>
    ///<para>当此数量大于物品槽的数量时，会使用余数筛选。</para>
    /// </param>
    public bool RemoveItemFromItemSlot(int itemSlotIndex, int number)
    {
        var count = _itemSlotNodes.Count;
        if (count == 0)
        {
            //Prevents the dividend from being 0
            //防止被除数为0
            return false;
        }

        var newIndex = itemSlotIndex % count;
        var itemSlot = _itemSlotNodes[newIndex];
        return itemSlot.RemoveItem(number);
    }

    /// <summary>
    /// <para>Select an item slot</para>
    /// <para>选中某个物品槽</para>
    /// </summary>
    private void SelectItemSlot(int oldSelectIndex, int newSelectIndex)
    {
        _itemSlotNodes[oldSelectIndex].IsSelect = false;
        _itemSlotNodes[newSelectIndex].IsSelect = true;
        var oldItem = _itemSlotNodes[oldSelectIndex].GetItem();
        if (oldItem != null && oldItem is Node2D oldNode2D)
        {
            oldNode2D.ProcessMode = ProcessModeEnum.Disabled;
            oldNode2D.Hide();
        }

        var item = _itemSlotNodes[newSelectIndex].GetItem();
        if (item == null)
        {
            LogCat.Log("选择" + oldSelectIndex + "新的为" + newSelectIndex + "空对象");
            GameSceneNodeHolder.Player.CurrentItem = null;
            LogCat.Log("我是空吗" + (GameSceneNodeHolder.Player.CurrentItem == null));
        }
        else
        {
            if (item is Node2D node2D)
            {
                LogCat.Log("我是空吗" + (GameSceneNodeHolder.Player.CurrentItem == null));
                LogCat.Log("选择" + oldSelectIndex + "新的为" + newSelectIndex + "已赋值" + node2D);
                node2D.ProcessMode = ProcessModeEnum.Inherit;
                node2D.Show();
                GameSceneNodeHolder.Player.CurrentItem = node2D;
            }
            else
            {
                GameSceneNodeHolder.Player.CurrentItem = null;
            }
        }
    }

    /// <summary>
    /// <para>Add an item to the HotBar</para>
    /// <para>在HotBar内添加一个物品</para>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool AddItem(IItem item)
    {
        return _itemSlotNodes.Count != 0 && _itemSlotNodes.Any(itemSlotNode => itemSlotNode.SetItem(item));
    }


    /// <summary>
    /// <para>Add items tank</para>
    /// <para>添加物品槽</para>
    /// </summary>
    private void AddItemSlot(int index)
    {
        var itemSlotNode = _itemSlotPackedScene.Instantiate() as ItemSlotNode;
        if (itemSlotNode == null)
        {
            return;
        }

        AddChild(itemSlotNode);
        itemSlotNode.IsSelect = index == selectIndex;
        _itemSlotNodes.Add(itemSlotNode);
    }
}