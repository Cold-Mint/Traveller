using System.Collections.Generic;
using ColdMint.scripts.item.weapon;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.item;

/// <summary>
/// <para>Item manager</para>
/// <para>物品管理器</para>
/// </summary>
public static class ItemTypeManager
{
    /// <summary>
    /// <para>Register items here</para>
    /// <para>在这里注册物品</para>
    /// </summary>
    public static void StaticRegister()
    {
        var staffOfTheUndeadScene = ResourceLoader.Load<PackedScene>("res://prefab/weapons/staffOfTheUndead.tscn");
        var staffOfTheUndeadIcon = ResourceLoader.Load<Texture2D>("res://sprites/weapon/staffOfTheUndead.png");
        var staffOfTheUndead =
            new ItemType("staff_of_the_undead",
                () => NodeUtils.InstantiatePackedScene<ProjectileWeapon>(staffOfTheUndeadScene), staffOfTheUndeadIcon,
                1);
        Register(staffOfTheUndead);

        var packsackScene = ResourceLoader.Load<PackedScene>("res://prefab/packsacks/packsack.tscn");
        var packsackIcon = ResourceLoader.Load<Texture2D>("res://sprites/Player.png");
        var packsack = new ItemType("packsack", () => NodeUtils.InstantiatePackedScene<Packsack>(packsackScene),
            packsackIcon, 1);
        Register(packsack);
    }

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
    /// <para>Returns null when the id is not registered.</para>
    /// </summary>
    public static IItem? NewItem(string id) =>
        Registry.TryGetValue(id, out var itemType) ? itemType.NewItemFunc() : null;

    /// <summary>
    /// Get the translated default name of the item type for the given id
    /// </summary>
    /// <returns>
    /// Translated default name of the item id if it exists. Else, return the id itself
    /// </returns>
    public static string DefaultNameOf(string id) => TranslationServerUtils.Translate($"item_{id}") ?? id;

    /// <summary>
    /// Get the translated default description of the item type for the given id
    /// </summary>
    /// <returns>
    /// Translated default description of the item id if it exists. Else, return null
    /// </returns>
    public static string? DefaultDescriptionOf(string id) => TranslationServerUtils.Translate($"item_{id}_desc");

    /// <summary>
    /// Get the default icon of the item type for the given id
    /// </summary>
    /// <returns>
    /// Translated default icon of the item id if it exists. Else, return a placeholder
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