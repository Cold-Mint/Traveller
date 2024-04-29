using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>A slot in the inventory</para>
/// <para>物品栏内的一个插槽</para>
/// </summary>
public partial class ItemSlotNode : MarginContainer
{
    private IItem? _item;
    private TextureRect _backgroundTextureRect;
    private TextureButton _iconTextureRect;
    private Label _quantityLabel;
    private Control _control;

    /// <summary>
    /// <para>Sets items for the item slot</para>
    /// <para>为物品槽设置物品</para>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool SetItem(IItem item)
    {
        if (_item == null)
        {
            if (item.Icon != null)
            {
                _iconTextureRect.TextureNormal = item.Icon;
            }

            _item = item;
            UpdateTooltipText(item);
            UpdateQuantityLabel(item.Quantity);
            return true;
        }
        else
        {
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
        _control.TooltipText = string.Format(TranslationServer.Translate("item_prompt_debug"), item.Id,
            TranslationServer.Translate(item.Name),
            item.Quantity, item.MaxStackQuantity, item.GetType().Name);
    }

    /// <summary>
    /// <para>Update quantity label</para>
    /// <para>更新数量标签</para>
    /// </summary>
    /// <param name="quantity"></param>
    private void UpdateQuantityLabel(int? quantity)
    {
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
        _backgroundTextureRect =
            GetNode<TextureRect>("BackgroundTexture");
        _iconTextureRect = GetNode<TextureButton>("BackgroundTexture/CenterContainer/IconTextureRect");
        _quantityLabel = GetNode<Label>("Control/QuantityLabel");
        _control = GetNode<Control>("Control");
        _quantityLabel.Visible = false;
    }
}