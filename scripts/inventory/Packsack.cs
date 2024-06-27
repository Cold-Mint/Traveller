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
    private PackedScene? _packedScene;
    private PacksackUi? _packsackUi;
    [Export] public int NumberSlots { get; set; }
    
    /// <summary>
    /// <para>Whether to allow backpacks</para>
    /// <para>是否允许放置背包</para>
    /// </summary>
    /// <remarks>
    ///<para>Can a new backpack be placed in the slot of the backpack?</para>
    ///<para>即此背包的槽位内是否可以再放置新的背包？</para>
    /// </remarks>
    [Export] public bool BackpackAllowed { get; set; }

    public override bool CanPutInPack => false;


    public override void Use(Node2D? owner, Vector2 targetGlobalPosition)
    {
        if (_packedScene == null)
        {
            return;
        }

        if (_packsackUi == null)
        {
            _packsackUi = NodeUtils.InstantiatePackedScene<PacksackUi>(_packedScene);
            if (_packsackUi != null)
            {
                NodeUtils.CallDeferredAddChild(NodeUtils.FindContainerNode(_packsackUi, this), _packsackUi);
                _packsackUi.Title = Name;
                _packsackUi.ItemContainer = ItemContainer;
            }
        }

        GameSceneNodeHolder.BackpackUiContainer?.Show();
        _packsackUi?.Show();
    }

    public IItemContainer? ItemContainer { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        ItemContainer = new UniversalItemContainer();
        ItemContainer.SupportSelect = false;
        //When the backpack is created, the item slot is generated.
        //当背包被创建时，物品槽就被生成出来了。
        for (var i = 0; i < NumberSlots; i++)
        {
            var itemSlotNode = ItemContainer.AddItemSlot(this);
            if (itemSlotNode == null)
            {
                continue;
            }

            itemSlotNode.BackpackAllowed = BackpackAllowed;
            itemSlotNode.Hide();
        }

        _packedScene = GD.Load<PackedScene>("res://prefab/ui/packsackUI.tscn");
    }
}