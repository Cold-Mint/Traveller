using ColdMint.scripts.inventory;
using ColdMint.scripts.item.itemStacks;
using Godot;

namespace ColdMint.scripts.item;

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
        if (ItemContainer == null) return;
        foreach (var itemSlot in ItemContainer)
        {
            itemSlot.ClearSlot();
        }

        QueueFree();
    }

    public bool CanStackWith(IItem item) => false;

    public IItemStack SpecialStack()
    {
        return new PacksackStack(this);
    }


    public IItemContainer? ItemContainer { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        ItemContainer = new UniversalItemContainer();
    }
}