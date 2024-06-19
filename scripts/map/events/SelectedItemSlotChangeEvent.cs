using ColdMint.scripts.inventory;

namespace ColdMint.scripts.map.events;

/// <summary>
/// <para>Selected item slot changes event</para>
/// <para>选中的物品槽改变事件</para>
/// </summary>
public class SelectedItemSlotChangeEvent
{
    /// <summary>
    /// <para></para>
    /// <para>新选中的物品槽</para>
    /// </summary>
    public ItemSlotNode? NewItemSlotNode { get; set; }
    
    /// <summary>
    /// <para>Lost the selected item slot</para>
    /// <para>失去选中的物品槽</para>
    /// </summary>
    public ItemSlotNode? OldItemSlotNode { get; set; }
}