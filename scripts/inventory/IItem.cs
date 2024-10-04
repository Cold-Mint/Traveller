using Godot;

namespace ColdMint.scripts.inventory;

public interface IItem
{
    /// <summary>
    /// <para>The position of the item in the container</para>
    /// <para>物品在容器内的位置</para>
    /// </summary>
    public int Index { get; set; }
    /// <summary>
    /// <para>ID of current item</para>
    /// <para>当前物品的ID</para>
    /// </summary>
    string Id { get; set; }
    
    /// <summary>
    /// <para>ShowSelf</para>
    /// <para>显示自身</para>
    /// </summary>
    void ShowSelf();

    /// <summary>
    /// <para>QueueFreeSelf</para>
    /// <para>销毁自身</para>
    /// </summary>
    void QueueFreeSelf();
    
    /// <summary>
    /// <para>HideSelf</para>
    /// <para>隐藏自身</para>
    /// </summary>
    void HideSelf();

    /// <summary>
    /// <para>Icon of current item</para>
    /// <para>当前项目的图标</para>
    /// </summary>
    Texture2D Icon { get; }

    /// <summary>
    /// <para>Display name of current item</para>
    /// <para>显示当前Item的名称</para>
    /// </summary>
    string Name { get; }

    /// <summary>
    /// <para>Description of current item, which may show in inventory</para>
    /// <para>当前项目的描述</para>
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// <para>Quantity</para>
    /// <para>当前的数量</para>
    /// </summary>
    int Quantity { get; set; }

    /// <summary>
    /// <para>MaxItemQuantity</para>
    /// <para>最大物品数量</para>
    /// </summary>
    int MaxQuantity { get; }
    
    /// <summary>
    /// <para>ItemType</para>
    /// <para>获取物品类型</para>
    /// </summary>
    int ItemType { get; }

    /// <summary>
    /// <para>Check or not</para>
    /// <para>是否选中</para>
    /// </summary>
    bool IsSelect { get; set; }
    
    /// <summary>
    /// <para>The container in which the item is located</para>
    /// <para>物品所在的物品容器</para>
    /// </summary>
    IItemContainer? ItemContainer { get; set; }
    
    /// <summary>
    /// <para>Its own container of items</para>
    /// <para>自身的物品容器</para>
    /// </summary>
    /// <remarks>
    /// <para>Returns a non-null value if the item itself can hold other items</para>
    /// <para>物品本身可容纳其他物品，则返回非空值</para>
    /// </remarks>
    public IItemContainer? SelfItemContainer { get; set; }

    /// <summary>
    /// <para>Calculate how many items can be merged with other items</para>
    /// <para>计算当前物品可与其他物品合并多少个</para>
    /// </summary>
    /// <param name="other"></param>
    /// <param name="unallocatedQuantity">
    ///<para>The amount yet to be allocated(This method doesn't actually change the number of iitems, so you need to allocate an int variable to keep track of how many items remain unallocated.)</para>
    ///<para>尚未分配的数量（在此方法并不会实际改变IItem的数量，故您需要分配一个int型变量记录还有多少个物品尚未分配）</para>
    /// </param>
    /// <returns>
    ///<para>Number of mergable numbers. 0 indicates that the number cannot be merged. Greater than 0 indicates that the number can be merged.</para>
    ///<para>可合并的数量，0为不能合并，大于0可合并。</para>
    /// </returns>
    int MergeableItemCount(IItem other, int unallocatedQuantity);

    /// <summary>
    /// <para>Create a new item instance</para>
    /// <para>创建新的物品实例</para>
    /// </summary>
    /// <param name="number">
    ///<para>Quantity (pass in a value less than 0 to create an instance using all the quantities of built-in items)</para>
    ///<para>数量（传入小于0的值，使用内置物品的所有数量创建实例）</para>
    /// </param>
    /// <returns>
    ///<para>Newly created item</para>
    ///<para>新创建的物品</para>
    /// </returns>
    IItem? CreateItem(int number);

    /// <summary>
    /// <para>Execute when current item is used <br/> e.g. when player clicks left mouse button with current item in hand</para>
    /// <para>当前项被使用时执行 <br/> e.g. 当玩家用鼠标左键点击当前物品时</para>
    /// </summary>
    /// <param name="owner">Owner of current item, if any</param>
    /// <param name="targetGlobalPosition">Target position, such as the position of the cursor when used by the player</param>
    void Use(Node2D? owner, Vector2 targetGlobalPosition);

    /// <summary>
    /// <para>When the item is thrown</para>
    /// <para>当物品被抛出时</para>
    /// </summary>
    /// <param name="velocity"></param>
    void OnThrow(Vector2 velocity);
}