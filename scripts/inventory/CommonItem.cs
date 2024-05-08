using System;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>Common goods</para>
/// <para>普通的物品</para>
/// </summary>
public class CommonItem : IItem
{
    public string? Id { get; set; }
    public int Quantity { get; set; }
    public int MaxStackQuantity { get; set; }
    public Texture2D? Icon { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Action<IItem>? OnUse { get; set; }
    public Func<IItem, Node>? OnInstantiation { get; set; }
}