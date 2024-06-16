using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>Backpack UI</para>
/// <para>背包UI</para>
/// </summary>
public partial class PacksackUi : Control
{
    private IItemContainer? _itemContainer;

    private PackedScene? _packedScene;

    private GridContainer? _gridContainer;

    /// <summary>
    /// <para>Packsack</para>
    /// <para>背包</para>
    /// </summary>
    public IItemContainer? ItemContainer
    {
        get => _itemContainer;
        set
        {
            _itemContainer = value;
            PlaceItemSlot(value);
        }
    }

    /// <summary>
    /// <para>Place item slots according to item information</para>
    /// <para>根据物品信息放置物品槽</para>
    /// </summary>
    /// <param name="itemContainer"></param>
    private void PlaceItemSlot(IItemContainer? itemContainer)
    {
        if (_gridContainer == null || itemContainer == null)
        {
            return;
        }

        NodeUtils.DeleteAllChild(_gridContainer);
        foreach (var itemSlotNode in itemContainer)
        {
            itemSlotNode.Reparent(_gridContainer);
        }
    }

    public override void _Ready()
    {
        _packedScene = GD.Load<PackedScene>("res://prefab/ui/ItemSlot.tscn");
        _gridContainer = GetNode<GridContainer>("GridContainer");
        _gridContainer.Columns = Config.HotBarSize;
        //If the item container was set before this node was placed in the node tree, load it here.
        //若物品容器在此节点放置到节点树之前被设置了，那么在这里加载。
        PlaceItemSlot(_itemContainer);
    }
}