using System;
using System.Collections.Generic;
using ColdMint.scripts.map.events;
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
public interface IItemContainer : IEnumerable<ItemSlotNode>
{
    /// <summary>
    /// <para>This event is triggered when the selected item slot changes</para>
    /// <para>当选中的物品槽改变时，触发此事件</para>
    /// </summary>
    Action<SelectedItemSlotChangeEvent>? SelectedItemSlotChangeEvent { get; set; }

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
    /// <para>Whether this item container supports checking</para>
    /// <para>此物品容器是否支持选中</para>
    /// </summary>
    public bool SupportSelect { get; set; }
    

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
    /// <para>Gets the item slot for the specified location, equals to <see cref="GetItemSlotNode"/></para>
    /// <para>获取指定位置的物品槽，等同于<see cref="GetItemSlotNode"/></para>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    ItemSlotNode? this[int index] => GetItemSlotNode(index);
    
    

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
    /// <para>AddItemSlot</para>
    /// <para>添加物品槽</para>
    /// </summary>
    /// <param name="rootNode"></param>
    ItemSlotNode? AddItemSlot(Node rootNode);

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