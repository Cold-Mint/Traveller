using System;
using System.Text.RegularExpressions;
using ColdMint.scripts.item;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>A slot in the inventory</para>
/// <para>物品栏内的一个插槽</para>
/// </summary>
public partial class ItemSlotNode : MarginContainer
{
    private TextureRect? _backgroundTextureRect;
    private TextureRect? _iconTextureRect;
    private Label? _quantityLabel;
    private Control? _control;
    private bool _isSelect;
    private Texture2D? _backgroundTexture;
    private Texture2D? _backgroundTextureWhenSelect;
    private IItem? _item;

    public IItem? Item
    {
        set
        {
            //Set item
            //设置物品
            _item = value;
            UpdateAllDisplay();
        }
    }

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
        if (_isSelect || _iconTextureRect == null)
        {
            //Drag is not allowed if there is no icon or no pile of items.
            //如果没有图标或者没有物品堆，那么不允许拖动。
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
        //If the preplaced slot does not have an icon, the preplaced slot is not allowed.
        //如果预放置的槽位没有图标，那么不允许放置。
        if (_iconTextureRect == null)
        {
            return false;
        }

        var type = data.VariantType;
        if (type == Variant.Type.Nil)
        {
            //The preplaced data is null.
            //预放置的数据为null。
            return false;
        }

        var itemSlotNode = data.As<ItemSlotNode>();
        var item = itemSlotNode.GetItem();
        if (item == null)
        {
            //Return null when trying to get the source item.
            //尝试获取源物品时返回null。
            return false;
        }

        return _item == null;
    }

    /// <summary>
    /// <para>Get the items in the item container</para>
    /// <para>获取物品容器内的物品</para>
    /// </summary>
    /// <returns>
    ///<para>There may be multiple quantities</para>
    ///<para>数量可能有多个</para>
    /// </returns>
    public IItem? GetItem()
    {
        return _item;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        if (_iconTextureRect == null)
        {
            return;
        }

        var type = data.VariantType;
        if (type == Variant.Type.Nil)
        {
            //The passed variable is null.
            //传入的变量为null。
            return;
        }

        var itemSlotNode = data.As<ItemSlotNode>();
        var item = itemSlotNode._item;
        if (item == null)
        {
            //Return null when trying to get the source item.
            //尝试获取源物品时返回null。
            return;
        }

        itemSlotNode.Item = null;
        Item = item;
    }

    public bool IsSelect
    {
        get => _isSelect;
        set
        {
            if (_backgroundTextureRect != null)
            {
                _backgroundTextureRect.Texture = value ? _backgroundTextureWhenSelect : _backgroundTexture;
            }

            _isSelect = value;
        }
    }

    public TextureRect? BackgroundTextureRect => _backgroundTextureRect;

    /// <summary>
    /// <para>Whether the item in this node is empty</para>
    /// <para>此节点内的物品是否为空的</para>
    /// </summary>
    /// <returns>
    ///<para>Return true if the number of items is 0 or the item object does not exist</para>
    ///<para>当物品数量为0或物品对象不存在时，返回true</para>
    /// </returns>
    public bool IsEmpty()
    {
        if (_item == null || _item.Quantity == 0)
        {
            return true;
        }

        return false;
    }


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
        if (_item == null)
        {
            TooltipText = null;
            return;
        }

        if (Config.IsDebug())
        {
            var debugText = TranslationServerUtils.Translate("item_prompt_debug");
            if (debugText != null)
            {
                TooltipText = string.Format(debugText, _item.Id,
                    TranslationServerUtils.Translate(_item.Name),
                    _item.Quantity, _item.MaxQuantity, _item.GetType().Name,
                    TranslationServerUtils.Translate(_item.Description));
            }
        }
        else
        {
            TooltipText = TranslationServerUtils.Translate(_item.Name) + "\n" +
                          TranslationServerUtils.Translate(_item.Description);
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

        switch (_item?.Quantity)
        {
            case null or 1:
                _quantityLabel.Hide();
                return;
            default:
                //When the quantity is not null or 1, we display the quantity.
                //当数量不为null或1时，我们显示数量
                _quantityLabel.Text = _item?.Quantity.ToString();
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
            _iconTextureRect.Texture = _item?.Icon;
        }
    }

    public bool CanAddItem(IItem item)
    {
        if (_item == null)
        {
            //If there is no item in the current item slot, it is allowed to add.
            //如果当前物品槽内没物品，那么允许添加。
            return true;
        }

        if (item.Id != _item.Id)
        {
            //If the item ID you want to add is different from the current item ID, disable.
            //如果要添加的物品ID和当前的物品ID不一样，那么禁止。
            return false;
        }

        var newQuantity = item.Quantity + _item.Quantity;
        if (newQuantity > _item.MaxQuantity)
        {
            //The maximum number is exceeded and items cannot be added.
            //超过了最大数量，无法添加物品。
            return false;
        }

        return true;
    }

    /// <summary>
    /// <para>Remove items from the item slot</para>
    /// <para>从物品槽内移除物品</para>
    /// </summary>
    /// <param name="number">
    ///<para>number(Less than 0, remove all items)</para>
    ///<para>物品数量(小于0，则移除全部物品)</para>
    /// </param>
    /// <returns>
    ///<para>How many items were actually removed</para>
    ///<para>实际移除了多少个物品</para>
    /// </returns>
    public int RemoveItem(int number)
    {
        if (_item == null)
        {
            return 0;
        }

        //The number of actual removals
        //实际移除的数量
        var removeNumber = number < 0 ? _item.Quantity : number;
        _item.Quantity -= removeNumber;
        if (_item.Quantity <= 0)
        {
            Item = null;
        }

        return removeNumber;
    }

    public bool AddItem(IItem item)
    {
        if (_item == null)
        {
            Item = item;
            return true;
        }

        var newQuantity = item.Quantity + _item.Quantity;
        _item.Quantity = Math.Min(newQuantity, _item.MaxQuantity);
        return true;
    }
}