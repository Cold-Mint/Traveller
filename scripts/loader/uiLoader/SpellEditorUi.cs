using ColdMint.scripts.inventory;
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
        _itemSlot.Update(_itemContainer.GetPlaceHolderItem(0));
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