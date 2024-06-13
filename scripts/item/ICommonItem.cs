using ColdMint.scripts.item.itemStacks;

namespace ColdMint.scripts.item;

/// <summary>
/// <para>Special item interface that make item common, which means will stack in a <see cref="CommonItemStack"/></para>
/// <para>该特殊的物品接口使得物品成为平凡物品，换言之，将会堆叠在<see cref="CommonItemStack"/></para>中。
/// </summary>
/// <typeparam name="TSelf">
/// <para>Make this the class itself</para>
/// <para>应当为当前类自身</para>
/// </typeparam>
/// <remarks>
/// <para>
///     Notice when you implement: To avoid unexpected behavior, unless you understand what you're doing, the <see cref="IItem.CanStackWith"/> method
///     of an item that implements the interface must only match its own exact same instance.
/// </para>
/// <para>实现时注意：为避免意外行为，除非你明白自己在做什么，否则实现接口的物品的<see cref="IItem.CanStackWith"/>方法必须只和自己完全相同的实例匹配。</para>
/// </remarks>
public interface ICommonItem : IItem
{
    /// <summary>
    /// <para>Method to copy an instance same with self. Will be used to pick out item instance from a <see cref="CommonItemStack"/></para>
    /// <para>复制与自身相同的实例的方法。将用于从 <see cref="CommonItemStack"/> 中拿取新的物品实例。</para>
    /// </summary>
    /// <returns></returns>
    ICommonItem CopyInstance();
}