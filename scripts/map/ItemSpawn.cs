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
    [Export] private string[]? _itemIdList;

    public Node2D? Spawn(int waveNumber)
    {
        if (_itemIdList == null)
        {
            return null;
        }

        if (waveNumber < 0 || waveNumber >= _itemIdList.Length)
        {
            return null;
        }
        var itemId = _itemIdList[waveNumber];
        if (string.IsNullOrEmpty(itemId))
        {
            return null;
        }
        var item = ItemTypeManager.CreateItem(itemId, this);
        LogCat.LogWithFormat("generated_item_is_empty", LogCat.LogLabel.ItemSpawn, itemId, item == null);
        if (item is not Node2D node2D)
        {
            return null;
        }
        node2D.GlobalPosition = GlobalPosition;
        return node2D;
    }

    public int GetMaxWaveNumber()
    {
        return _itemIdList?.Length ?? 0;
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