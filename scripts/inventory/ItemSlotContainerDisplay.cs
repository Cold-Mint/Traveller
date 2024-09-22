using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.inventory;

public class ItemSlotContainerDisplay(Node rootNode) : ItemContainerDisplayTemplate
{
    private readonly PackedScene? _packedScene = GD.Load<PackedScene>("res://prefab/ui/ItemSlot.tscn");

    protected override void AddItemDisplay()
    {
        if (_packedScene == null)
        {
            return;
        }

        var itemSlotNode = NodeUtils.InstantiatePackedScene<ItemSlotNode>(_packedScene);
        if (itemSlotNode == null)
        {
            return;
        }

        ItemDisplayList.Add(itemSlotNode);
        NodeUtils.CallDeferredAddChild(rootNode, itemSlotNode);
    }
}