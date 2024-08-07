﻿using System;
using Godot;

namespace ColdMint.scripts.inventory;

public readonly struct ItemType(
    string id,
    Texture2D? icon,
    int maxStackQuantity,
    Func<Node?, IItem?> createItemFunc
)
{
    /// <summary>
    /// <para>Item id of this type</para>
    /// <para>该类型物品的id</para>
    /// </summary>
    public string Id { get; init; } = id;

    /// <summary>
    /// <para>A function returns a new item instance of this type</para>
    /// <para>用于创建该类型的物品实例的函数</para>
    /// </summary>
    public Func<Node?, IItem?> CreateItemFunc { get; init; } = createItemFunc;

    /// <summary>
    /// <para>Default icon of items of this type</para>
    /// <para>该类型物品的默认图标</para>
    /// </summary>
    public Texture2D? Icon { get; init; } = icon;

    /// <summary>
    /// <para>Max number in item stack of this type</para>
    /// <para>该类型物品的最大堆叠数量</para>
    /// </summary>
    public int MaxStackQuantity { get; init; } = maxStackQuantity;
}