using System.Collections.Generic;
using System.Linq;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>Item manager</para>
/// <para>物品管理器</para>
/// </summary>
public static class ItemTypeManager
{
    private static readonly Dictionary<string, ItemTypeInfo> Registry = [];
    private static readonly Dictionary<int, List<string>> TypeCodeToIds = [];

    public static ItemTypeInfo[] GetAllItemType() => Registry.Values.ToArray();

    public static string[] GetAllIds(int typeCode = Config.ItemTypeCode.All) =>
        TypeCodeToIds.TryGetValue(typeCode, out var ids) ? ids.ToArray() : [];

    /// <summary>
    /// <para>Get a random item Id</para>
    /// <para>获取随机的物品Id</para>
    /// </summary>
    /// <returns></returns>
    public static string? GetRandomId(int typeCode = Config.ItemTypeCode.All)
    {
        if (!TypeCodeToIds.TryGetValue(typeCode, out var ids))
        {
            return null;
        }

        return ids.Count == 0
            ?
            //No item of that type has been registered.
            //没有对应类型的物品被注册了。
            null
            : ids[GD.RandRange(0, ids.Count - 1)];
    }

    /// <summary>
    /// <para>Register an item type.</para>
    /// <para>Return false if the item id already exist.</para>
    /// <para>注册一个物品类型</para>
    /// <para>如果项目id已经存在，则返回false。</para>
    /// </summary>
    /// <returns><para>Whether the registration was successful.</para>
    /// <para>注册是否成功。</para>
    /// </returns>
    public static bool Register(ItemTypeInfo itemType)
    {
        if (!Registry.TryAdd(itemType.Id, itemType))
        {
            return false;
        }

        if (TypeCodeToIds.TryGetValue(itemType.TypeCode, out var ids))
        {
            ids.Add(itemType.Id);
        }
        else
        {
            TypeCodeToIds.Add(itemType.TypeCode, [itemType.Id]);
        }

        if (TypeCodeToIds.TryGetValue(Config.ItemTypeCode.All, out var allIds))
        {
            allIds.Add(itemType.Id);
        }
        else
        {
            TypeCodeToIds.Add(Config.ItemTypeCode.All, [itemType.Id]);
        }

        return true;
    }

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
    /// <param name="applyNewItemGlow">
    ///<para>Apply luminescence effect</para>
    ///<para>应用发光效果</para>
    /// </param>
    /// <seealso cref="CreateItems"/>
    public static IItem? CreateItem(string id, Node? defaultParentNode = null, bool applyNewItemGlow = true)
    {
        if (!Registry.TryGetValue(id, out var itemType))
        {
            return null;
        }

        var newItem = NodeUtils.InstantiatePackedScene<IItem>(GD.Load<PackedScene>(itemType.ScenePath));
        if (newItem is not Node node) return newItem;
        if (defaultParentNode == null)
        {
            node.QueueFree();
            return null;
        }

        newItem.Id = itemType.Id;
        if (applyNewItemGlow)
        {
            newItem.ApplyNewItemGlow();
        }

        NodeUtils.CallDeferredAddChild(NodeUtils.FindContainerNode(node, defaultParentNode), node);
        return newItem;
    }


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
    /// <para>Whether an item is included</para>
    /// <para>是否包含某个物品</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool Contains(string id) => Registry.ContainsKey(id);

    /// <summary>
    /// <para>Obtain the icon based on the ID</para>
    /// <para>根据ID获取对应的图标</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Texture2D? GetIconOf(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        return Registry.TryGetValue(id, out var itemType) ? GD.Load<Texture2D>(itemType.IconPath) : null;
    }
}