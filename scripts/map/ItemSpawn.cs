using ColdMint.scripts.debug;
using ColdMint.scripts.inventory;
using ColdMint.scripts.map.room;
using Godot;

namespace ColdMint.scripts.map;

/// <summary>
/// <para>ItemSpawn</para>
/// <para>物品出生点</para>
/// </summary>
public partial class ItemSpawn : Marker2D, ISpawnMarker
{
    [Export] private string? ItemId { get; set; }

    public Node2D? Spawn()
    {
        if (string.IsNullOrEmpty(ItemId))
        {
            return null;
        }

        var item = ItemTypeManager.CreateItem(ItemId, this);
        LogCat.LogWithFormat("generated_item_is_empty", LogCat.LogLabel.ItemSpawn, true, ItemId, item == null);
        if (item is not Node2D node2D)
        {
            return null;
        }
        node2D.GlobalPosition = GlobalPosition;
        return node2D;
    }

    public bool CanQueueFree()
    {
        return true;
    }

    public void DoQueueFree()
    {
        QueueFree();
    }
}