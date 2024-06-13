using System;

using ColdMint.scripts.debug;
using ColdMint.scripts.item;

using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>packsack</para>
/// <para>背包</para>
/// </summary>
public partial class Packsack : RigidBody2D, IItem
{
    [Export] public string Id { get; protected set; } = "place_holder_id";

    protected Texture2D? UniqueIcon { get; set; }
    public Texture2D Icon => UniqueIcon ?? ItemTypeManager.DefaultIconOf(Id);

    protected string? UniqueName { get; set; }
    public new string Name => UniqueName ?? ItemTypeManager.DefaultNameOf(Id);

    protected string? UniqueDescription { get; set; }
    public string? Description => UniqueDescription ?? ItemTypeManager.DefaultDescriptionOf(Id);

    public void Use(Node2D? owner, Vector2 targetGlobalPosition) { }

    public void Destroy()
    {
        if (_itemContainer == null) return;
        foreach (var itemSlot in _itemContainer)
        {
            itemSlot.ClearSlot();
        }

        QueueFree();
    }

    public bool CanStackWith(IItem item) => false;

    private IItemContainer? _itemContainer;

    public override void _Ready()
    {
        base._Ready();
        _itemContainer = new UniversalItemContainer();
    }

    public IItemContainer? GetItemContainer()
    {
        return _itemContainer;
    }
}