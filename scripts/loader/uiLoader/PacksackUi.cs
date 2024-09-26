using ColdMint.scripts.inventory;
using Godot;

namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>Backpack UI</para>
/// <para>背包UI</para>
/// </summary>
public partial class PacksackUi : UiLoaderTemplate
{
    private IItemContainer? _itemContainer;

    private PackedScene? _packedScene;

    private HFlowContainer? _hFlowContainer;

    private Label? _titleLabel;

    private string? _title;

    private Button? _exitButton;

    private IItemContainerDisplay? _itemContainerDisplay;

    /// <summary>
    /// <para>title</para>
    /// <para>标题</para>
    /// </summary>
    public string? Title
    {
        get => _title;
        set
        {
            _title = value;
            SetTile(value);
        }
    }

    /// <summary>
    /// <para>Packsack</para>
    /// <para>背包</para>
    /// </summary>
    public IItemContainer? ItemContainer
    {
        get => _itemContainer;
        set
        {
            _itemContainer = value;
            BindItemContainer();
        }
    }


    /// <summary>
    /// <para>SetTile</para>
    /// <para>设置标题</para>
    /// </summary>
    /// <param name="title"></param>
    private void SetTile(string? title)
    {
        if (_titleLabel == null)
        {
            return;
        }

        _titleLabel.Text = title;
    }

    public override void InitializeData()
    {
        _packedScene = GD.Load<PackedScene>("res://prefab/ui/ItemSlot.tscn");
    }

    private void BindItemContainer()
    {
        if (ItemContainer == null)
        {
            return;
        }
        _itemContainerDisplay?.BindItemContainer(ItemContainer);
    }

    public override void InitializeUi()
    {
        _hFlowContainer = GetNode<HFlowContainer>("HFlowContainer");
        _titleLabel = GetNode<Label>("TitleLabel");
        _exitButton = GetNode<Button>("ExitButton");
        _itemContainerDisplay = new ItemSlotContainerDisplay(_hFlowContainer);
        //If the item container was set before this node was placed in the node tree, load it here.
        //若物品容器在此节点放置到节点树之前被设置了，那么在这里加载。
        BindItemContainer();
        SetTile(_title);
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