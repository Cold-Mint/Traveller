using ColdMint.scripts.item.itemStacks;

namespace ColdMint.scripts.map.events;

/// <summary>
/// <para>ItemStackChangeEvent</para>
/// <para>物品堆改变事件</para>
/// </summary>
public class ItemStackChangeEvent
{
    /// <summary>
    /// <para>Changed ItemStack</para>
    /// <para>改变后的物品堆</para>
    /// </summary>
    public IItemStack? ItemStack { get; set; }
}