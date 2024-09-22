using ColdMint.scripts.pickable;
using ColdMint.scripts.utils;
using Godot;
using PacksackUi = ColdMint.scripts.loader.uiLoader.PacksackUi;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>packsack</para>
/// <para>背包</para>
/// </summary>
public partial class Packsack : PickAbleTemplate
{
    private const string Path = "res://prefab/ui/packsackUI.tscn";
    [Export] public int NumberSlots { get; set; }

    /// <summary>
    /// <para>Whether to allow backpacks</para>
    /// <para>是否允许放置背包</para>
    /// </summary>
    /// <remarks>
    ///<para>Can a new backpack be placed in the slot of the backpack?</para>
    ///<para>即此背包的槽位内是否可以再放置新的背包？</para>
    /// </remarks>
    [Export]
    public bool BackpackAllowed { get; set; }

    public override bool CanPutInPack => false;


    public override void Use(Node2D? owner, Vector2 targetGlobalPosition)
    {
        GameSceneDepend.DynamicUiGroup?.ShowControl(Path, control =>
        {
            if (control is PacksackUi packsackUi)
            {
                packsackUi.Title = Name;
                packsackUi.ItemContainer = ItemContainer;
            }
        });
    }

    public IItemContainer? ItemContainer { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        ItemContainer = new UniversalItemContainer(NumberSlots);
        ItemContainer.SupportSelect = false;
        GameSceneDepend.DynamicUiGroup?.RegisterControl(Path, () =>
        {
            var packedScene = GD.Load<PackedScene>(Path);
            return NodeUtils.InstantiatePackedScene<PacksackUi>(packedScene);
        });
    }
}