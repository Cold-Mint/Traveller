using System;
using ColdMint.scripts.map.events;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>item container</para>
/// <para>物品容器</para>
/// </summary>
public interface IItemContainer
{
    /// <summary>
    /// <para>This event is triggered when the selected item changes</para>
    /// <para>当选中的物品改变时，触发此事件</para>
    /// </summary>
    Action<SelectedItemChangeEvent>? SelectedItemChangeEvent { get; set; }

    /// <summary>
    /// <para>This event is triggered when the item's data changes, such as the number increases, decreases, or new items are added to the container</para>
    /// <para>当物品的数据发生改变时，例如数量增加，减少，或者新物品被添加到容器内触发此事件</para>
    /// </summary>
    Action<ItemDataChangeEvent>? ItemDataChangeEvent { get; set; }
    
    /// <summary>
    /// <para>Allow Adding Item By Type</para>
    /// <para>允许添加指定类型的物品</para>
    /// </summary>
    /// <param name="itemType"></param>
    void AllowAddingItemByType(int itemType);
    
    /// <summary>
    /// <para>Disallow Adding Item By Type</para>
    /// <para>禁止添加指定类型的物品</para>
    /// </summary>
    /// <param name="itemType"></param>
    void DisallowAddingItemByType(int itemType);

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
    /// <returns>
    ///<para>How many items were successfully added. The addition failed, and 0 was returned.</para>
    ///<para>有多少个物品被成功添加了。添加失败，返回0</para>
    /// </returns>
    int AddItem(IItem item);

    /// <summary>
    /// <para>Whether this item container supports checking</para>
    /// <para>此物品容器是否支持选中</para>
    /// </summary>
    bool SupportSelect { get; set; }
    
    /// <summary>
    /// <para>Gets a placeholder object</para>
    /// <para>获取占位符对象</para>
    /// </summary>
    /// <param name="index">
    ///<para>index</para>
    ///<para>占位符代替的索引</para>
    /// </param>
    /// <returns></returns>
    IItem GetPlaceHolderItem(int index);

    /// <summary>
    /// <para>Gets the selected location</para>
    /// <para>获取选中的位置</para>
    /// </summary>
    /// <returns></returns>
    int GetSelectIndex();

    /// <summary>
    /// <para>Gets the currently selected item</para>
    /// <para>获取当前选中的物品</para>
    /// </summary>
    /// <returns></returns>
    IItem? GetSelectItem();

    /// <summary>
    /// <para>Gets the item in the specified location</para>
    /// <para>获取指定位置的物品</para>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IItem? GetItem(int index);

    /// <summary>
    /// <para>ReplaceItem</para>
    /// <para>替换物品</para>
    /// </summary>
    /// <remarks>
    ///<para>Even if the item corresponding to the index is null, it can be successfully replaced.</para>
    ///<para>即使索引对应的物品为null，也可以成功的替换。</para>
    /// </remarks>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool ReplaceItem(int index, IItem item);
    
    /// <summary>
    /// <para>Whether items are replaceable</para>
    /// <para>是否可替换物品</para>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool CanReplaceItem(int index, IItem item);

    /// <summary>
    /// <para>ClearItem</para>
    /// <para>清理物品</para>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    bool ClearItem(int index);
    
    /// <summary>
    /// <para>ClearAllItems</para>
    /// <para>清理全部物品</para>
    /// </summary>
    void ClearAllItems();

    /// <summary>
    /// <para>Gets the item in the specified location, equivalent to <see cref="GetItem"/></para>
    /// <para>获取指定位置的物品，等同于<see cref="GetItem"/></para>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IItem? this[int index] => GetItem(index);


    /// <summary>
    /// <para>Removes the item from the currently selected location</para>
    /// <para>移除当前选中位置的物品</para>
    /// </summary>
    /// <param name="number">
    /// <para>Quantity to be removed, inputs below zero represent all items</para>
    /// <para>要删除的数量，小于0的输入代表全部物品</para>
    /// </param>
    /// <returns>
    ///<para>How many items were actually removed</para>
    ///<para>实际移除了多少个物品</para>
    /// </returns>
    int RemoveSelectItem(int number);

    /// <summary>
    /// <para>Deduct the number of items in the specified location</para>
    /// <para>扣除指定位置的物品数量</para>
    /// </summary>
    /// <param name="itemIndex"></param>
    /// <param name="number">
    /// <para>Quantity to be removed, inputs below zero represent all items</para>
    /// <para>要删除的数量，小于0的输入代表全部物品</para>
    /// </param>
    /// <returns>
    ///<para>How many items were actually removed</para>
    ///<para>实际移除了多少个物品</para>
    /// </returns>
    int RemoveItem(int itemIndex, int number);

    /// <summary>
    /// <para>Gets the used capacity of the item container</para>
    /// <para>获取物品容器已使用的容量</para>
    /// </summary>
    /// <returns></returns>
    int GetUsedCapacity();

    /// <summary>
    /// <para>Gets the total capacity of the item container</para>
    /// <para>获取物品容器的总容量</para>
    /// </summary>
    /// <returns></returns>
    int GetTotalCapacity();

    /// <summary>
    /// <para>Select the next item</para>
    /// <para>选择下一个物品</para>
    /// </summary>
    void SelectNextItem();

    /// <summary>
    /// <para>Select the previous item</para>
    /// <para>选择上一个物品</para>
    /// </summary>
    void SelectPreviousItem();

    /// <summary>
    /// <para>选择物品</para>
    /// <para>Select item</para>
    /// </summary>
    /// <param name="index"></param>
    void SelectItem(int index);
}