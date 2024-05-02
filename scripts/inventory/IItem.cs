using System;
using Godot;

namespace ColdMint.scripts.inventory;

public interface IItem
{
    /// <summary>
    /// <para>Item and ID</para>
    /// <para>物品还有ID</para>
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// <para>Represents the quantity of this item</para>
    /// <para>表示此物品的数量</para>
    /// </summary>
    int Quantity { get; set; }

    /// <summary>
    /// <para>How many can this item stack up to</para>
    /// <para>这个物品最多叠加到多少个</para>
    /// </summary>
    int MaxStackQuantity { get; set; }

    /// <summary>
    /// <para>Items can be set with Icon</para>
    /// <para>物品可以设置图标</para>
    /// </summary>
    Texture2D Icon { get; set; }

    /// <summary>
    /// <para>Item has a name</para>
    /// <para>物品有名称</para>
    /// </summary>
    string Name { get; set; }
    
    /// <summary>
    /// <para>Description</para>
    /// <para>描述</para>
    /// </summary>
    string Description { get; set; }


    /// <summary>
    /// <para>When using items</para>
    /// <para>当使用物品时</para>
    /// </summary>
    Action<IItem> OnUse { get; set; }

    /// <summary>
    /// <para>When removing items from the backpack, instantiate them</para>
    /// <para>当从背包内取出，实例化物品时</para>
    /// </summary>
    Func<IItem, Node> OnInstantiation { get; set; }
}