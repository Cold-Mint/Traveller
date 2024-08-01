using System.Collections.Generic;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>Item manager</para>
/// <para>物品管理器</para>
/// </summary>
public static class ItemTypeManager
{
    private static Dictionary<string, ItemType> Registry { get; } = [];
    private static Texture2D DefaultTexture { get; } = new PlaceholderTexture2D();


    /// <summary>
    /// <para>Register an item type.</para>
    /// <para>Return false if the item id already exist.</para>
    /// <para>注册一个物品类型</para>
    /// <para>如果项目id已经存在，则返回false。</para>
    /// </summary>
    /// <returns><para>Whether the registration was successful.</para>
    /// <para>注册是否成功。</para>
    /// </returns>
    public static bool Register(ItemType itemType) => Registry.TryAdd(itemType.Id, itemType);

    /// <summary>
    /// <para>Creates a new instance of the item registered to the given id.</para>
    /// <para>创建给定物品id的新物品实例</para>
    /// </summary>
    /// <returns>
    /// <para>Returns null when the id is not registered.</para>
    /// <para>当物品id没有注册时返回null</para>
    /// </returns>
    /// <param name="id">
    ///<para>Item Id</para>
    ///<para>物品Id</para>
    /// </param>
    /// <param name="defaultParentNode">
    ///<para>Default parent</para>
    ///<para>父节点</para>
    /// </param>
    /// <seealso cref="CreateItems"/>
    public static IItem? CreateItem(string id, Node? defaultParentNode = null) =>
        Registry.TryGetValue(id, out var itemType)
            ? itemType.CreateItemFunc(defaultParentNode)
            : null;


    /// <summary>
    /// <para>Create multiple new item instances for the given item ID</para>
    /// <para>创建多个给定物品Id的新物品实例</para>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="number"></param>
    /// <param name="defaultParentNode"></param>
    /// <param name="globalPosition"></param>
    /// <returns></returns>
    /// <seealso cref="CreateItem"/>
    public static IItem?[]? CreateItems(string id, int number, Vector2 globalPosition, Node? defaultParentNode = null)
    {
        if (number <= 0)
        {
            return null;
        }

        var items = new List<IItem?>();
        for (var i = 0; i < number; i++)
        {
            var singleItem = CreateItem(id, defaultParentNode);
            if (singleItem == null)
            {
                continue;
            }

            if (singleItem is Node2D node2D)
            {
                node2D.GlobalPosition = globalPosition;
            }

            items.Add(singleItem);
        }

        return items.ToArray();
    }
    

    /// <summary>
    /// <para>Get the default icon of the item type for the given id</para>
    /// <para>获取指定物品id的默认图标</para>
    /// </summary>
    /// <returns>
    /// <para>Default icon of the item id if it exists. Else, return a <see cref="PlaceholderTexture2D"/></para>
    /// <para>当前物品id的默认图标，若无则返回一个<see cref="PlaceholderTexture2D"/></para>
    /// </returns>
    public static Texture2D DefaultIconOf(string id) =>
        Registry.TryGetValue(id, out var itemType)
            ? itemType.Icon ?? DefaultTexture
            : DefaultTexture;

    /// <summary>
    /// <para>Gets the maximum number of stacks for an item</para>
    /// <para>获取某个物品的最大堆叠数量</para>
    /// </summary>
    /// <param name="id">
    ///<para>id</para>
    ///<para>物品ID</para>
    /// </param>
    /// <returns></returns>
    public static int MaxStackQuantityOf(string id) =>
        Registry.TryGetValue(id, out var itemType) ? itemType.MaxStackQuantity : 0;
}