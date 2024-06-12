using System;
using System.Diagnostics.CodeAnalysis;

using Godot;

namespace ColdMint.scripts.item;

/// <summary>
/// <para>Item stack in an inventory slot</para>
/// </summary>
public interface IItemStack
{
    /// <summary>
    /// <para>ID of items inside current stack</para>
    /// </summary>
    string Id { get; }

    /// <summary>
    /// <para>Max number of current stack</para>
    /// </summary>
    int MaxQuantity { get; }

    /// <summary>
    /// <para>Quantity of current stack</para>
    /// </summary>
    int Quantity { get; }

    /// <summary>
    /// <para>Icon of current item</para>
    /// </summary>
    Texture2D Icon { get; }

    /// <summary>
    /// <para>Display name of current item</para>
    /// </summary>
    string Name { get; }

    /// <summary>
    /// <para>Description of current item, which may show in inventory</para>
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// <para>Determine whether a specified item can be accommodated</para>
    /// <para>判断能否容纳指定物品</para>
    /// </summary>
    /// <returns></returns>
    public bool CanAddItem(IItem item);

    /// <summary>
    /// <para>Hold a given item</para>
    /// </summary>
    /// <param name="item">Item to hold by current stack</param>
    /// <returns>Whether successful</returns>
    public bool AddItem(IItem item);

    /// <summary>
    /// <para>判断能从指定物品堆中接收的物品数量</para>
    /// </summary>
    /// <param name="itemStack">
    /// <para>向该物品堆中放入物品的物品堆</para>
    /// <para>Item stack to add to the current stack</para>
    /// </param>
    /// <returns></returns>
    public int CanTakeFrom(IItemStack itemStack);

    /// <summary>
    /// <para>将指定物品堆中尽可能多的物品移动至当前物品堆中，被移入当前堆的物品应从原物品堆中移除。</para>
    /// </summary>
    /// <param name="itemStack">
    /// <para>被移入当前堆的物品堆</para>
    /// </param>
    /// <returns>
    /// <para>操作结束后原物品堆是否为空</para>
    /// </returns>
    public bool TakeFrom(IItemStack itemStack);

    /// <summary>
    /// <para>Get item instance at the top of current stack without removing it from stack</para>
    /// <para>获取当前物品堆顶部的物品实例而不取出该物品</para>
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
    /// <para>Clear current stack, which means should dispose all items inside current stack here</para>
    /// </summary>
    public void ClearStack();

    /// <summary>
    /// Create a new ItemStack with the given item as the first item
    /// </summary>
    public static IItemStack FromItem(IItem item) => ItemTypeManager.MaxStackQuantityOf(item.Id) switch
    {
        1         => new SingleItemStack(item),
        > 1       => throw new NotImplementedException(),
        var other => throw new ArgumentException($"item {item} of type '{item.Id}' has unexpected max stack quantity {other}")
    };
}