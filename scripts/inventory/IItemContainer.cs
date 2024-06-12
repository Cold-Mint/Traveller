using System;
using System.Collections;
using System.Collections.Generic;

using ColdMint.scripts.item;

using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>item container</para>
/// <para>物品容器</para>
/// </summary>
/// <remarks>
///<para>Item containers can store items. Things like backpacks and Hotbars are containers with visual pages.</para>
///<para>物品容器可以储存物品。像背包和hotbar是具有可视化页面的容器。</para>
/// </remarks>
public interface IItemContainer
{
    /// <summary>
    /// <para>Can the specified item be added to the container?</para>
    /// <para>指定的物品是否可添加到容器内？</para>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool CanAddItem(IItem item);

    /// <summary>
    /// <para>Implement methods for adding items</para>
    /// <para>实现添加物品的方法</para>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool AddItem(IItem item);

    /// <summary>
    /// <para>Add an stack of items to this container</para>
    /// <para>向当前容器中存入一堆物品</para>
    /// </summary>
    /// <param name="itemStack"></param>
    /// <returns>
    /// <para>If the source item stack is empty after the operation is completed</para>
    /// <para>操作完成后，源物品堆是否被取空</para>
    /// </returns>
    bool AddItemStack(IItemStack itemStack);

    /// <summary>
    /// <para>Gets the selected location</para>
    /// <para>获取选中的位置</para>
    /// </summary>
    /// <returns></returns>
    int GetSelectIndex();

    /// <summary>
    /// <para>Gets the currently selected node</para>
    /// <para>获取当前选中的节点</para>
    /// </summary>
    /// <returns></returns>
    ItemSlotNode? GetSelectItemSlotNode();
    
    /// <summary>
    /// <para>If present, remove an item from the slot at the currently selected location and return it.</para>
    /// <para>如果存在，移除当前选中位置的槽位中的一个物品并将其返回</para>
    /// </summary>
    /// <seealso cref="PickItemFromItemSlot"/><seealso cref="PickItemsFromItemSlotBySelectIndex"/>
    IItem? PickItemFromItemSlotBySelectIndex();

    /// <summary>
    /// <para>Remove the specified number of items from the item slot at the currently selected location, and return them as a new item stack</para>
    /// <para>取出当前选中位置的物品槽中指定数量的物品，并作为新的物品堆返回</para>
    /// </summary>
    /// <param name="value">
    /// <para>Quantity to be taken out, inputs below zero represent all items</para>
    /// <para>要取出的数量，小于0的输入代表全部物品</para>
    /// </param>
    /// <seealso cref="PickItemsFromItemSlot"/><seealso cref="PickItemFromItemSlotBySelectIndex"/>
    IItemStack? PickItemsFromItemSlotBySelectIndex( int value);

    /// <summary>
    /// <para>Removes an item from the inventory at the currently selected location</para>
    /// <para>移除当前选中位置物品栏内的物品</para>
    /// </summary>
    /// <param name="number">
    /// <para>Quantity to be removed, inputs below zero represent all items</para>
    /// <para>要删除的数量，小于0的输入代表全部物品</para>
    /// </param>
    /// <returns>
    /// <para>The remaining number, if the number of items in the current item stack is less than the specified number. Otherwise,0</para>
    /// <para>若物品槽内物品少于指定的数量，返回相差的数量。否则返回0</para>
    /// </returns>
    /// <remarks>
    /// <para>Will remove the removed items from the game, if that is not the intent, consider using the <see cref="PickItemsFromItemSlotBySelectIndex"/></para>
    /// <para>会将移除的物品从游戏中删除，如果目的并非如此，请考虑使用<see cref="PickItemsFromItemSlotBySelectIndex"/></para>
    /// </remarks>
    int RemoveItemFromItemSlotBySelectIndex(int number);

    /// <summary>
    /// <para>Gets the number of item slots</para>
    /// <para>获取物品槽的数量</para>
    /// </summary>
    /// <returns></returns>
    int GetItemSlotCount();

    /// <summary>
    /// <para>Gets the item slot for the specified location</para>
    /// <para>获取指定位置的物品槽</para>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    ItemSlotNode? GetItemSlotNode(int index);

    /// <summary>
    /// <para>If present, remove an item from the slot in the specified location and return it.</para>
    /// <para>如果存在，移除指定位置的槽位中的一个物品并将其返回</para>
    /// </summary>
    /// <seealso cref="PickItemsFromItemSlot"/>
    IItem? PickItemFromItemSlot(int itemSlotIndex);

    /// <summary>
    /// <para>Remove the specified number of items from the item slot in the specified location, and return them as a new item stack</para>
    /// <para>取出指定位置的物品槽中指定数量的物品，并作为新的物品堆返回</para>
    /// </summary>
    /// <param name="itemSlotIndex"></param>
    /// <param name="value">
    /// <para>Quantity to be taken out, inputs below zero represent all items</para>
    /// <para>要取出的数量，小于0的输入代表全部物品</para>
    /// </param>
    /// <seealso cref="PickItemFromItemSlot"/>
    IItemStack? PickItemsFromItemSlot(int itemSlotIndex, int value);
    
    /// <summary>
    /// <para>Removes an item from the item slot in the specified location</para>
    /// <para>在指定位置的物品槽内移除物品</para>
    /// </summary>
    /// <param name="itemSlotIndex"></param>
    /// <param name="number">
    /// <para>Quantity to be removed, inputs below zero represent all items</para>
    /// <para>要删除的数量，小于0的输入代表全部物品</para>
    /// </param>
    /// <returns>
    /// <para>The remaining number, if the number of items in the current item stack is less than the specified number. Otherwise,0</para>
    /// <para>若物品槽内物品少于指定的数量，返回相差的数量。否则返回0</para>
    /// </returns>
    /// <remarks>
    /// <para>Will remove the removed items from the game, if that is not the intent, consider using the <see cref="PickItemsFromItemSlot"/></para>
    /// <para>会将移除的物品从游戏中删除，如果目的并非如此，请考虑使用<see cref="PickItemsFromItemSlot"/></para>
    /// </remarks>
    int RemoveItemFromItemSlot(int itemSlotIndex, int number);

    /// <summary>
    /// <para>Based on the given item, match the item slots where it can be added to </para>
    /// <para>根据给定的物品，匹配可放置它的物品槽</para>
    /// </summary>
    /// <param name="item"></param>
    /// <returns>
    /// <para>Return null if there is no slot to place the item in</para>
    /// <para>若没有槽可放置此物品，则返回null</para>
    /// </returns>
    ItemSlotNode? Match(IItem item);

    /// <summary>
    /// <para>Based on the given item stack, match the item slots where it can be added to</para>
    /// <para>根据给定的物品堆，匹配可放置它的物品槽</para>
    /// </summary>
    /// <param name="stack"></param>
    /// <returns>
    /// <para>Return null if there is no slot to add the item slot in</para>
    /// <para>若没有槽可放置此物品堆，则返回null</para>
    /// </returns>
    ItemSlotNode? Match(IItemStack stack);

    /// <summary>
    /// <para>Match the first item slot that has item stack that satisfies the predicate</para>
    /// <para>匹配首个拥有满足指定条件的物品堆的物品槽</para>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>
    /// <para>Return null if there is no slot satisfies the predicate</para>
    /// <para>若没有满足条件的槽位，返回null</para>
    /// </returns>
    /// <seealso cref="MatchAll"/>
    ItemSlotNode? Match(Func<IItemStack?, bool> predicate);

    /// <summary>
    /// <para>Match all item slots that has item stack that satisfies the predicate</para>
    /// <para>匹配所有拥有满足指定条件的物品堆的物品槽</para>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>
    /// <para>IEnumerable for the item slot matched to, will be empty if there's no slot satisfies the predicate</para>
    /// <para>包含匹配到的槽位的IEnumerable，当没有满足条件的槽位时为空</para>
    /// </returns>
    /// <seealso cref="Match(Func{IItemStack?,bool})"/>
    IEnumerable<ItemSlotNode> MatchAll(Func<IItemStack?, bool> predicate);


    /// <summary>
    /// <para>AddItemSlot</para>
    /// <para>添加物品槽</para>
    /// </summary>
    /// <param name="rootNode"></param>
    /// <param name="index"></param>
    void AddItemSlot(Node rootNode, int index);

    /// <summary>
    /// <para>SelectTheNextItemSlot</para>
    /// <para>选择下一个物品槽</para>
    /// </summary>
    void SelectTheNextItemSlot();

    /// <summary>
    /// <para>SelectThePreviousItemSlot</para>
    /// <para>选择上一个物品槽</para>
    /// </summary>
    void SelectThePreviousItemSlot();

    /// <summary>
    /// <para>选择物品槽</para>
    /// <para>SelectItemSlot</para>
    /// </summary>
    /// <param name="newSelectIndex"></param>
    void SelectItemSlot(int newSelectIndex);
}