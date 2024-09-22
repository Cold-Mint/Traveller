using ColdMint.scripts.inventory;

namespace ColdMint.scripts.map.events;

/// <summary>
/// <para>Selected item slot changes event</para>
/// <para>选中的物品槽改变事件</para>
/// </summary>
public class SelectedItemChangeEvent
{
    public int NewIndex { get; set; }
    
    public int OldIndex { get; set; }
    
    /// <summary>
    /// <para>Newly selected item</para>
    /// <para>新选中的物品</para>
    /// </summary>
    public IItem? NewItem { get; set; }
    
    /// <summary>
    /// <para>Lost the selected item</para>
    /// <para>失去选中的物品</para>
    /// </summary>
    public IItem? OldItem { get; set; }
}