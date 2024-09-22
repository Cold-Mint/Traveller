using ColdMint.scripts.inventory;
using ColdMint.scripts.utils;
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
            PlaceItemSlot(value);
        }
    }

    /// <summary>
    /// <para>Place item slots according to item information</para>
    /// <para>根据物品信息放置物品槽</para>
    /// </summary>
    /// <param name="itemContainer"></param>
    private void PlaceItemSlot(IItemContainer? itemContainer)
    {
        if (_hFlowContainer == null || itemContainer == null)
        {
            return;
        }

        NodeUtils.DeleteAllChild(_hFlowContainer);
        //todo:实现使用物品数据刷新物品槽的方法。
        // foreach (var item in itemContainer)
        // {
            // itemSlotNode.Reparent(_hFlowContainer);
            // itemSlotNode.Show();
        // }
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

    public override void InitializeUi()
    {
        _hFlowContainer = GetNode<HFlowContainer>("HFlowContainer");
        _titleLabel = GetNode<Label>("TitleLabel");
        _exitButton = GetNode<Button>("ExitButton");
        //If the item container was set before this node was placed in the node tree, load it here.
        //若物品容器在此节点放置到节点树之前被设置了，那么在这里加载。
        PlaceItemSlot(_itemContainer);
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