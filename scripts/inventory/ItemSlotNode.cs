using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>A slot in the inventory</para>
/// <para>物品栏内的一个插槽</para>
/// </summary>
public partial class ItemSlotNode : MarginContainer
{
    private IItem? _item;
    private TextureRect? _backgroundTextureRect;
    private TextureRect? _iconTextureRect;
    private Label? _quantityLabel;
    private Control? _control;
    private bool _isSelect;
    private Texture2D? _backgroundTexture;
    private Texture2D? _backgroundTextureWhenSelect;

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
    /// <para>Get the items in the item slot</para>
    /// <para>获取物品槽内的物品</para>
    /// </summary>
    /// <returns></returns>
    public IItem? GetItem()
    {
        return _item;
    }

    /// <summary>
    /// <para>Removes the specified number of items from the item slot</para>
    /// <para>在物品槽内移除指定数量的物品</para>
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public bool RemoveItem(int number)
    {
        if (_item == null)
        {
            return false;
        }

        var newNumber = _item.Quantity - number;
        if (newNumber <= 0)
        {
            //If the specified number of items is removed, the number of items is less than or equal to 0. Then we return the removal successful and empty the inventory.
            //如果移除指定数量的物品后，物品数量小于或等于0。那么我们返回移除成功，并清空物品栏。
            ClearItem();
            return true;
        }
        else
        {
            _item.Quantity = newNumber;
            UpdateTooltipText(_item);
            UpdateQuantityLabel(_item.Quantity);
            return true;
        }
    }

    /// <summary>
    /// <para>Empty the items in the item slot</para>
    /// <para>清空物品槽内的物品</para>
    /// </summary>
    /// <remarks>
    ///<para>This method does not calculate how many items should be left. If you want to remove a specified number of items, call the RemoveItem method.</para>
    ///<para>此方法不会计算物品应该剩余多少个。若您希望移除指定数量的物品，请调用RemoveItem方法。</para>
    /// </remarks>
    public void ClearItem()
    {
        _item = null;
        if (_iconTextureRect != null)
        {
            _iconTextureRect.Texture = null;
        }

        if (_control != null)
        {
            _control.TooltipText = null;
        }

        if (_quantityLabel != null)
        {
            _quantityLabel.Visible = false;
        }
    }


    /// <summary>
    /// <para>Can the specified item be placed in the item slot?</para>
    /// <para>指定的物品是否可设置在物品槽内？</para>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool CanSetItem(IItem item)
    {
        if (_item == null)
        {
            return true;
        }

        //This inventory already has items, but the items in this inventory are not the same as the incoming items
        //这个物品栏已经有物品了，但是这个物品栏的物品和传入的物品不一样
        if (_item.Id != item.Id)
        {
            return false;
        }

        var newQuantity = _item.Quantity + item.Quantity;
        if (newQuantity > item.MaxStackQuantity)
        {
            //If the amount of the current item exceeds the maximum stack amount after placing it in this inventory
            //如果将当前物品放置到这个物品栏后，数量超过了最大叠加数量
            return false;
        }

        return true;
    }

    /// <summary>
    /// <para>Sets items for the item slot</para>
    /// <para>为物品槽设置物品</para>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool SetItem(IItem item)
    {
        if (!CanSetItem(item))
        {
            return false;
        }

        if (_item == null)
        {
            if (item.Icon != null && _iconTextureRect != null)
            {
                _iconTextureRect.Texture = item.Icon;
            }

            _item = item;
            UpdateTooltipText(item);
            UpdateQuantityLabel(item.Quantity);
            return true;
        }
        else
        {
            var newQuantity = _item.Quantity + item.Quantity;
            _item.Quantity = newQuantity;
            UpdateTooltipText(item);
            UpdateQuantityLabel(newQuantity);
            return true;
        }
    }

    /// <summary>
    /// <para>Update item tips</para>
    /// <para>更新物品的提示内容</para>
    /// </summary>
    /// <param name="item"></param>
    private void UpdateTooltipText(IItem item)
    {
        if (_control == null)
        {
            return;
        }

        if (Config.IsDebug())
        {
            _control.TooltipText = string.Format(TranslationServer.Translate("item_prompt_debug"), item.Id,
                TranslationServer.Translate(item.Name),
                item.Quantity, item.MaxStackQuantity, item.GetType().Name,
                TranslationServer.Translate(item.Description));
        }
        else
        {
            _control.TooltipText = TranslationServer.Translate(item.Name) + "\n" +
                                   TranslationServer.Translate(item.Description);
        }
    }

    /// <summary>
    /// <para>Update quantity label</para>
    /// <para>更新数量标签</para>
    /// </summary>
    /// <param name="quantity"></param>
    private void UpdateQuantityLabel(int? quantity)
    {
        if (_quantityLabel == null)
        {
            return;
        }

        switch (quantity)
        {
            case null:
                _quantityLabel.Visible = false;
                return;
            case > 1:
                //When the quantity is greater than 1, we display the quantity.
                //当数量大于1时，我们显示数量
                _quantityLabel.Text = quantity.ToString();
                _quantityLabel.Visible = true;
                break;
            default:
                _quantityLabel.Visible = false;
                break;
        }
    }

    public override void _Ready()
    {
        _backgroundTexture = GD.Load<Texture2D>("res://sprites/ui/ItemBarEmpty.png");
        _backgroundTextureWhenSelect = GD.Load<Texture2D>("res://sprites/ui/ItemBarFocus.png");
        _backgroundTextureRect =
            GetNode<TextureRect>("BackgroundTexture");
        _iconTextureRect = GetNode<TextureRect>("BackgroundTexture/CenterContainer/IconTextureRect");
        _quantityLabel = GetNode<Label>("Control/QuantityLabel");
        _control = GetNode<Control>("Control");
        _quantityLabel.Visible = false;
    }
}