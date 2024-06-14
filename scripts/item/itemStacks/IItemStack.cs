using System;
using Godot;

namespace ColdMint.scripts.item.itemStacks;

/// <summary>
/// <para>物品槽中的物品堆</para>
/// <para>Item stack in an inventory slot</para>
/// </summary>
public interface IItemStack
{
    /// <summary>
    /// <para>Max number of current stack</para>
    /// <para>当前物品堆的最大物品数量</para>
    /// </summary>
    int MaxQuantity { get; }

    /// <summary>
    /// <para>Quantity of current stack</para>
    /// <para>当前物品堆的物品数量</para>
    /// </summary>
    int Quantity { get; }

    /// <summary>
    /// <para>True if this stack is empty</para>
    /// <para>当物品堆空时为真</para>
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This attribute is used to check if the item stack is empty after the operation for subsequent processing,<br/>
    ///     i.e. there should not be any item stacks with this attribute true outside of the operation process
    /// </para>
    /// <para>此属性用于检查操作后该物品堆是否为空以便后续处理，也就是说在操作过程以外的时候不应当存在任何该属性为true的物品堆</para>
    /// </remarks>
    bool Empty { get; }

    /// <summary>
    /// <para>Icon of current item stack</para>
    /// <para>当前物品堆显示的图标</para>
    /// </summary>
    Texture2D Icon { get; }

    /// <summary>
    /// <para>Display name of current item stack</para>
    /// <para>当前物品堆显示的名称</para>
    /// </summary>
    string Name { get; }

    /// <summary>
    /// <para>Description of current item stack, which may show in inventory</para>
    /// <para>当前物品堆的描述，可能显示在物品栏中</para>
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// <para>Determine whether a specified item can be accommodated</para>
    /// <para>判断能否容纳指定物品</para>
    /// </summary>
    /// <returns></returns>
    public bool CanAddItem(IItem item);

    /// <summary>
    /// <para>Add items to the itemStack</para>
    /// <para>添加物品到物品堆内</para>
    /// </summary>
    /// <param name="item">
    ///<para>Items to add</para>
    ///<para>需要添加的物品</para>
    /// </param>
    /// <returns>
    ///<para>Whether successful</para>
    ///<para>是否成功</para>
    /// </returns>
    public bool AddItem(IItem item);

    /// <summary>
    /// <para>Determines the number of items that can be received from the specified pile</para>
    /// <para>判断能从指定物品堆中接收的物品数量</para>
    /// </summary>
    /// <param name="itemStack">
    /// <para>向该物品堆中放入物品的物品堆</para>
    /// <para>Item stack to add to the current stack</para>
    /// </param>
    /// <returns></returns>
    public int CanTakeFrom(IItemStack itemStack);

    /// <summary>
    /// <para>Move as many items as possible from the specified item pile to the current item pile. Items that have been moved to the current item pile should be removed from the original item pile.</para>
    /// <para>将指定物品堆中尽可能多的物品移动至当前物品堆中，被移入当前堆的物品应从原物品堆中移除。</para>
    /// </summary>
    /// <param name="itemStack">
    /// <para>The pile of items that are moved into the current pile</para>
    /// <para>被移入当前堆的物品堆</para>
    /// </param>
    /// <returns>
    /// <para>Whether the original stack is empty after the operation</para>
    /// <para>操作结束后原物品堆是否为空</para>
    /// </returns>
    public bool TakeFrom(IItemStack itemStack);

    /// <summary>
    /// <para>Gets an item instance of the current item pile without retrieving the item</para>
    /// <para>获取当前物品堆的物品实例而不取出该物品</para>
    /// <seealso cref="PickItem"/>
    /// </summary>
    /// <returns></returns>
    public IItem? GetItem();

    /// <summary>
    /// <para>Pop the item instance at the top of current item stack and return it</para>
    /// <para>取出当前物品堆顶部的物品实例并返回该物品</para>
    /// <seealso cref="GetItem"/><seealso cref="PickItems"/>
    /// </summary>
    /// <returns></returns>
    public IItem? PickItem();

    /// <summary>
    /// <para>Remove the specified number of items and return them as a new item stack</para>
    /// <para>取出当前堆中指定数量的物品，并作为新的物品堆返回</para>
    /// <seealso cref="PickItem"/>
    /// </summary>
    /// <param name="value">
    /// <para>Quantity to be taken out, inputs below zero represent all items</para>
    /// <para>要取出的数量，小于0的输入代表全部物品</para>
    /// </param>
    /// <returns>
    /// <para>The item stack that is taken out, can be null if out nothing, should not be the current item stack itself</para>
    /// <para>取出的物品堆，没有取出物品时可为null，不应是当前物品堆自身</para>
    /// </returns>
    public IItemStack? PickItems(int value);

    /// <summary>
    /// <para>
    /// Removes the specified number of items from current item stack,removed items should be removed from the game<br/>
    /// If you don't want remove them from game, consider <see cref="PickItem"/> and <see cref="PickItems"/>
    /// </para>
    /// <para>
    /// 在当前物品堆移除指定数量的物品，被移除的物品应当从游戏中移除。<br/>
    /// 如果您并不打算将它们从游戏中移除，请考虑使用 <see cref="PickItem"/> 和 <see cref="PickItems"/>
    /// </para>
    /// </summary>
    /// <param name="number">
    /// <para>Quantity to be removed, inputs below zero represent all items</para>
    /// <para>要删除的数量，小于0的输入代表全部物品</para>
    /// </param>
    /// <returns>
    /// <para>The remaining number, if the number of items in the current item stack is less than the specified number. Otherwise,0</para>
    /// <para>若物品槽内物品少于指定的数量，返回相差的数量。否则返回0</para>
    /// </returns>
    public int RemoveItem(int number);

    /// <summary>
    /// <para>Clear current stack, which means should remove all items inside current stack from the game here</para>
    /// <para>清除当前物品堆，意味着从游戏中移除当前堆中的所有物品</para>。
    /// </summary>
    public void ClearStack();

    /// <summary>
    /// <para>Create a new ItemStack with the given item as the first item</para>
    /// <para>以给定的物品为第一个物品创建物品堆</para>
    /// </summary>
    /// <remarks>
    ///<para>Assuming the item implements the <see cref="IItem.SpecialStack"/> method, then use the return value of the SpecialStack method, otherwise extrapolate from the maximum number of stacks of items.</para>
    ///<para>假设物品实现了<see cref="IItem.SpecialStack"/>方法，那么使用SpecialStack方法的返回值，否则根据物品的最大堆叠数量来推断。</para>
    /// </remarks>
    public static IItemStack FromItem(IItem item) =>
        item.SpecialStack() ??
        ItemTypeManager.MaxStackQuantityOf(item.Id) switch
        {
            1 => new SingleItemStack(item),
            > 1 => item is ICommonItem commonItem ? new CommonItemStack(commonItem) : new UniqueItemStack(item),
            var other => throw new ArgumentException(
                $"item {item} of type '{item.Id}' has unexpected max stack quantity {other}")
        };
}