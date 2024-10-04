using ColdMint.scripts.debug;
using ColdMint.scripts.inventory;
using ColdMint.scripts.map.events;
using Godot;

namespace ColdMint.scripts.map;

/// <summary>
/// <para>ItemSpawn</para>
/// <para>物品出生点</para>
/// </summary>
public partial class ItemSpawn : Marker2D
{
    [Export] public string? ItemId { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        EventBus.MapGenerationCompleteEvent += MapGenerationCompleteEvent;
    }

    private void MapGenerationCompleteEvent(MapGenerationCompleteEvent mapGenerationCompleteEvent)
    {
        //After the map is generated, create the item instance.
        //当地图生成完成后，创建物品实例。
        if (ItemId == null)
        {
            return;
        }

        var item = ItemTypeManager.CreateItem(ItemId, this);
        LogCat.LogWithFormat("generated_item_is_empty",LogCat.LogLabel.ItemSpawn,true,ItemId,item == null);
        if (item is Node2D node2D)
        {
            node2D.GlobalPosition = GlobalPosition;
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventBus.MapGenerationCompleteEvent -= MapGenerationCompleteEvent;
    }
}