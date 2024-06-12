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
    bool CanAddItem(IItem_New item);

    /// <summary>
    /// <para>Implement methods for adding items</para>
    /// <para>实现添加物品的方法</para>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool AddItem(IItem_New item);

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
    /// <param name="number"></param>
    /// <returns></returns>
    bool RemoveItemFromItemSlotBySelectIndex(int number);

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
    /// <para>Removes an item from the item slot in the specified location</para>
    /// <para>在指定位置的物品槽内移除物品</para>
    /// </summary>
    /// <param name="itemSlotIndex"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    bool RemoveItemFromItemSlot(int itemSlotIndex, int number);

    /// <summary>
    /// <para>Based on the given item, match the item slots where it can be placed</para>
    /// <para>根据给定的物品，匹配可放置它的物品槽</para>
    /// </summary>
    /// <param name="item"></param>
    /// <returns>
    ///<para>Return null if there is no slot to place the item in</para>
    ///<para>若没有槽可放置此物品，则返回null</para>
    /// </returns>
    ItemSlotNode? Matching(IItem_New item);

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