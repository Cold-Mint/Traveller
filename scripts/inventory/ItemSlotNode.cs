using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>A slot in the inventory</para>
/// <para>物品栏内的一个插槽</para>
/// </summary>
public partial class ItemSlotNode : MarginContainer, IItemDisplay
{
    private TextureRect? _backgroundTextureRect;
    private TextureRect? _iconTextureRect;
    private Label? _quantityLabel;
    private Control? _control;
    private Texture2D? _backgroundTexture;
    private Texture2D? _backgroundTextureWhenSelect;
    public IItem? Item { get; private set; }


    public override void _Ready()
    {
        _backgroundTexture = GD.Load<Texture2D>("res://sprites/ui/ItemBarEmpty.png");
        _backgroundTextureWhenSelect = GD.Load<Texture2D>("res://sprites/ui/ItemBarFocus.png");
        _backgroundTextureRect =
            GetNode<TextureRect>("BackgroundTexture");
        _iconTextureRect = GetNode<TextureRect>("BackgroundTexture/IconTextureRect");
        _quantityLabel = GetNode<Label>("Control/QuantityLabel");
        _control = GetNode<Control>("Control");
        _quantityLabel.Hide();
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        if (_iconTextureRect == null)
        {
            return new Variant();
        }

        var textureRect = new TextureRect();
        textureRect.ExpandMode = _iconTextureRect.ExpandMode;
        textureRect.Size = _iconTextureRect.Size;
        textureRect.Texture = _iconTextureRect.Texture;
        SetDragPreview(textureRect);
        return Variant.CreateFrom(this);
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        if (Item == null)
        {
            return false;
        }
        if (Item is PlaceholderItem)
        {
            return false;
        }
        return true;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        var type = data.VariantType;
        if (type == Variant.Type.Nil)
        {
            //The passed variable is null.
            //传入的变量为null。
            return;
        }
        var itemSlotNode = data.As<ItemSlotNode>();
        var sourceItem = itemSlotNode.Item;
        if (sourceItem == null)
        {
            //Return null when trying to get the source item.
            //尝试获取源物品时返回null。
            return;
        }
    }

    /// <summary>
    /// <para>Whether to place a backpack in the current slot</para>
    /// <para>当前槽位是否允许放置背包</para>
    /// </summary>
    public bool BackpackAllowed { get; set; }


    private void UpdateBackground(bool isSelect)
    {
        if (_backgroundTextureRect == null)
        {
            return;
        }

        _backgroundTextureRect.Texture = isSelect ? _backgroundTextureWhenSelect : _backgroundTexture;
    }

    public TextureRect? BackgroundTextureRect => _backgroundTextureRect;


    /// <summary>
    /// <para>Update all displays of this slot</para>
    /// <para>更新该槽位的一切显示信息</para>
    /// </summary>
    private void UpdateAllDisplay()
    {
        UpdateIconTexture();
        UpdateQuantityLabel();
        UpdateTooltipText();
    }

    /// <summary>
    /// <para>Update item tips</para>
    /// <para>更新物品的提示内容</para>
    /// </summary>
    private void UpdateTooltipText()
    {
        if (Item is PlaceholderItem or null)
        {
            TooltipText = null;
            return;
        }

        if (Config.IsDebug())
        {
            var debugText = TranslationServerUtils.Translate("item_prompt_debug");
            if (debugText != null)
            {
                TooltipText = string.Format(debugText, Item.Id,
                    TranslationServerUtils.Translate(Item.Name),
                    Item.Quantity, Item.MaxQuantity, Item.GetType().Name,
                    TranslationServerUtils.Translate(Item.Description));
            }
        }
        else
        {
            TooltipText = TranslationServerUtils.Translate(Item.Name) + "\n" +
                          TranslationServerUtils.Translate(Item.Description);
        }
    }

    /// <summary>
    /// <para>Update quantity label</para>
    /// <para>更新数量标签</para>
    /// </summary>
    private void UpdateQuantityLabel()
    {
        if (_quantityLabel == null)
        {
            return;
        }

        if (Item is PlaceholderItem or null)
        {
            _quantityLabel.Hide();
            return;
        }

        switch (Item?.Quantity)
        {
            case null or 1:
                _quantityLabel.Hide();
                return;
            default:
                //When the quantity is not null or 1, we display the quantity.
                //当数量不为null或1时，我们显示数量
                _quantityLabel.Text = Item?.Quantity.ToString();
                _quantityLabel.Show();
                break;
        }
    }


    /// <summary>
    /// <para>Update texture of the icon rect</para>
    /// <para>更新显示的物品图标</para>
    /// </summary>
    private void UpdateIconTexture()
    {
        if (_iconTextureRect != null)
        {
            _iconTextureRect.Texture = Item?.Icon;
        }
    }

    public void Update(IItem? item)
    {
        Item = item;
        UpdateAllDisplay();
        UpdateBackground(item is { IsSelect: true });
    }

    public void ShowSelf()
    {
        Show();
    }

    public void HideSelf()
    {
        Hide();
    }
}