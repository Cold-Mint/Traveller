using System;
using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>packsack</para>
/// <para>背包</para>
/// </summary>
public partial class Packsack : RigidBody2D, IItem
{
    public string? Id { get; set; }
    public int Quantity { get; set; }
    public int MaxStackQuantity { get; set; }
    public Texture2D? Icon { get; set; }
    public new string? Name { get; set; }
    public string? Description { get; set; }
    public Action<IItem>? OnUse { get; set; }
    public Func<IItem, Node>? OnInstantiation { get; set; }

    private IItemContainer? _itemContainer;

    public override void _Ready()
    {
        base._Ready();
        Id = GetMeta("ID", "1").AsString();
        Quantity = GetMeta("Quantity", "1").AsInt32();
        MaxStackQuantity = GetMeta("MaxStackQuantity", Config.MaxStackQuantity).AsInt32();
        Icon = GetMeta("Icon", "").As<Texture2D>();
        Name = GetMeta("Name", "").AsString();
        Description = GetMeta("Description", "").AsString();
        _itemContainer = new UniversalItemContainer();
    }

    public IItemContainer? GetItemContainer()
    {
        return _itemContainer;
    }
}