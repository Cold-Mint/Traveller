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
    public override void Use(Node2D? owner, Vector2 targetGlobalPosition)
    {
        GameSceneDepend.DynamicUiGroup?.ShowControl(Path, control =>
        {
            if (control is PacksackUi packsackUi)
            {
                packsackUi.Title = Name;
                packsackUi.ItemContainer = SelfItemContainer;
            }
        });
    }

    public override void CopyAttributes(Node node)
    {
        base.CopyAttributes(node);
        if (node is Packsack packsack)
        {
            SelfItemContainer = packsack.SelfItemContainer;
        }
    }


    public override void _Ready()
    {
        base._Ready();
        if (SelfItemContainer == null)
        {
            SelfItemContainer = new UniversalItemContainer(NumberSlots);
            SelfItemContainer.SupportSelect = false;
        }
        GameSceneDepend.DynamicUiGroup?.RegisterControl(Path, () =>
        {
            var packedScene = GD.Load<PackedScene>(Path);
            return NodeUtils.InstantiatePackedScene<PacksackUi>(packedScene);
        });
    }
}