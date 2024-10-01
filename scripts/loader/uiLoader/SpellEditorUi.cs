using ColdMint.scripts.inventory;
using ColdMint.scripts.map.events;
using Godot;

namespace ColdMint.scripts.loader.uiLoader;

public partial class SpellEditorUi : UiLoaderTemplate
{
    private Button? _exitButton;
    private IItemContainer? _itemContainer;
    private ItemSlotNode? _itemSlot;

    public override void InitializeUi()
    {
        _exitButton = GetNode<Button>("ExitButton");
        _itemSlot = GetNode<ItemSlotNode>("ItemSlot");
        _itemContainer = new UniversalItemContainer(1);
        _itemContainer.ItemDataChangeEvent += OnItemDataChangeEvent;
        _itemSlot.Update(_itemContainer.GetPlaceHolderItem(0));
    }

    private void OnItemDataChangeEvent(ItemDataChangeEvent itemDataChangeEvent)
    {
        if (_itemSlot == null)
        {
            return;
        }
        _itemSlot.Update(itemDataChangeEvent.NewItem);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_itemContainer != null)
        {
            _itemContainer.ItemDataChangeEvent -= OnItemDataChangeEvent;
        }
    }

    public override void LoadUiActions()
    {
        if (_exitButton != null)
        {
            _exitButton.Pressed += () =>
            {
                GameSceneDepend.DynamicUiGroup?.HideControl(this);
            };
        }
    }
}