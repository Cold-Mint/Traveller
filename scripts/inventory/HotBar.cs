using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ColdMint.scripts.inventory;

public partial class HotBar : HBoxContainer
{
    private PackedScene _itemSlotPackedScene;
    private List<ItemSlotNode> _itemSlotNodes;

    public override void _Ready()
    {
        base._Ready();
        _itemSlotNodes = new List<ItemSlotNode>();
        _itemSlotPackedScene = GD.Load<PackedScene>("res://prefab/ui/ItemSlot.tscn");
        for (int i = 0; i < 10; i++)
        {
            AddItemSlot();
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
    private void AddItemSlot()
    {
        var node = _itemSlotPackedScene.Instantiate();
        AddChild(node);
        _itemSlotNodes.Add(node as ItemSlotNode);
    }
}