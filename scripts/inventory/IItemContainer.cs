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
    /// <para>Gets the currently selected node</para>
    /// <para>获取当前选中的节点</para>
    /// </summary>
    /// <returns></returns>
    ItemSlotNode? GetSelectItemSlotNode();

    /// <summary>
    /// <para>Based on the given item, match the item slots where it can be placed</para>
    /// <para>根据给定的物品，匹配可放置它的物品槽</para>
    /// </summary>
    /// <param name="item"></param>
    /// <returns>
    ///<para>Return null if there is no slot to place the item in</para>
    ///<para>若没有槽可放置此物品，则返回null</para>
    /// </returns>
    ItemSlotNode? Matching(IItem item);
}