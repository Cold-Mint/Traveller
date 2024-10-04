using ColdMint.scripts.inventory;
using ColdMint.scripts.map.events;
using Godot;

namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>SpellEditorUi</para>
/// <para>法术编辑器UI</para>
/// </summary>
public partial class SpellEditorUi : UiLoaderTemplate
{
    private Button? _exitButton;
    private IItemContainer? _projectileWeaponContainer;
    private ItemSlotNode? _itemSlot;
    private HFlowContainer? _flowContainer;
    private IItemContainerDisplay? _itemContainerDisplay;

    public override void InitializeUi()
    {
        _exitButton = GetNode<Button>("ExitButton");
        _itemSlot = GetNode<ItemSlotNode>("ItemSlot");
        _projectileWeaponContainer = new UniversalItemContainer(1);
        _projectileWeaponContainer.AllowAddingItemByType(Config.ItemType.ProjectileWeapon);
        _projectileWeaponContainer.ItemDataChangeEvent += OnItemDataChangeEvent;
        _itemSlot.Update(_projectileWeaponContainer.GetPlaceHolderItem(0));
        _flowContainer = GetNode<HFlowContainer>("HFlowContainer");
        _itemContainerDisplay = new ItemSlotContainerDisplay(_flowContainer);
    }

    private void OnItemDataChangeEvent(ItemDataChangeEvent itemDataChangeEvent)
    {
        if (_itemSlot == null || _projectileWeaponContainer == null)
        {
            return;
        }
        var item = itemDataChangeEvent.NewItem;
        if (item == null)
        {
            item = _projectileWeaponContainer.GetPlaceHolderItem(itemDataChangeEvent.NewIndex);
        }
        item.IsSelect = false;
        _itemSlot.Update(item);
        _itemContainerDisplay?.BindItemContainer(item.SelfItemContainer);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_projectileWeaponContainer != null)
        {
            _projectileWeaponContainer.ItemDataChangeEvent -= OnItemDataChangeEvent;
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