using ColdMint.scripts.inventory;

namespace ColdMint.scripts.map.events;

/// <summary>
/// <para>Item Data Change Event</para>
/// <para>项目数据改变事件</para>
/// </summary>
public class ItemDataChangeEvent
{
    public Config.ItemDataChangeEventType Type { get; set; }
    public int OldIndex { get; set; }
    public int NewIndex { get; set; }
    public IItem? NewItem { get; set; }
    public IItem? OldItem { get; set; }
}