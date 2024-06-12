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
    //private IItem? _item;
    private IItemStack? _itemStack;
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
    /// <para>Get the item stack in the item slot</para>
    /// <para>获取物品槽内的物品堆</para>
    /// </summary>
    /// <returns></returns>
    public IItemStack? GetItemStack() => _itemStack;

    /// <summary>
    /// <para>If present, get the item at the top of the item stack in this slot</para>
    /// <para>如果存在，获取该槽位中物品堆顶部的物品</para>
    /// </summary>
    public IItem_New? GetItem() => _itemStack?.GetItem();

    /// <summary>
    /// <para>If present, remove an item in this slot and return it.</para>
    /// <para>如果存在，移除该槽位中的一个物品并将其返回</para>
    /// </summary>
    /// <seealso cref="PickItems"/>
    public IItem_New? PickItem()
    {
        if (_itemStack is null) return null;

        var result = _itemStack.PickItem();
        if (_itemStack.Quantity == 0) ClearSlot();
        else UpdateAllDisplay();

        return result;
    }

    /// <summary>
    /// <para>Remove the specified number of items and return them as a new item stack</para>
    /// <para>取出当前物品槽中指定数量的物品，并作为新的物品堆返回</para>
    /// </summary>
    /// <param name="value">
    /// <para>Quantity to be taken out, inputs below zero represent all items</para>
    /// <para>要取出的数量，小于0的输入代表全部物品</para>
    /// </param>
    /// <seealso cref="PickItem"/>
    public IItemStack? PickItems(int value)
    {
        if (_itemStack is null) return null;

        var result = _itemStack.PickItems(value);
        if (_itemStack.Quantity == 0) ClearSlot();
        else UpdateAllDisplay();

        return result;
    }

    /// <summary>
    /// <para>Removes the specified number of items from the item slot</para>
    /// <para>在物品槽内移除指定数量的物品</para>
    /// </summary>
    /// <param name="number">
    /// <para>Quantity to be removed, inputs below zero represent all items</para>
    /// <para>要删除的数量，小于0的输入代表全部物品</para>
    /// </param>
    /// <returns>
    /// <para>The remaining number, if the number of items in the current item stack is less than the specified number. Otherwise,0</para>
    /// <para>若物品槽内物品少于指定的数量，返回相差的数量。否则返回0</para>
    /// </returns>
    /// <remarks>
    /// <para>Will remove the removed items from the game, if that is not the intent, consider using the <see cref="PickItems"/></para>
    /// <para>会将移除的物品从游戏中删除，如果目的并非如此，请考虑使用<see cref="PickItems"/></para>
    /// </remarks>
    public int RemoveItem(int number)
    {
        if (_itemStack == null)
        {
            return number;
        }

        var result = _itemStack.RemoveItem(number);
        //If the specified number of items is removed, the number of items is less than or equal to 0. Then we empty the inventory.
        //如果移除指定数量的物品后，物品数量小于或等于0。那么我们清空物品栏。
        if (_itemStack.Quantity == 0) ClearSlot();
        else UpdateAllDisplay();

        return result;
    }

    /// <summary>
    /// <para>Remove item stack from slot and return it, equivalent to ReplaceItemStack(null)</para>
    /// <para>从当前槽位中移出并返回物品堆，等价于ReplaceItemStack(null)</para>
    /// <seealso cref="ReplaceItemStack"/>
    /// </summary>
    public IItemStack? RemoveItemStack() => ReplaceItemStack(null);

    /// <summary>
    /// <para>Empty the item slot</para>
    /// <para>清空当前物品槽</para>
    /// </summary>
    /// <remarks>
    ///<para>This method will remove all items stored in the item slots from the game, if this is not what you want to do, consider using the <see cref="RemoveItemStack"/> method.</para>
    ///<para>此方法会从游戏中移除储存于物品槽中的所有物品，若这不是您希望的操作，请考虑使用<see cref="RemoveItemStack"/>方法。</para>
    /// </remarks>
    public void ClearSlot()
    {
        _itemStack?.ClearStack();
        _itemStack = null;

        UpdateAllDisplay();
    }

    /// <summary>
    /// <para>Can the specified item be placed in the item slot?</para>
    /// <para>指定的物品是否可设置在物品槽内？</para>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool CanAddItem(IItem_New item)
    {
        if (_itemStack == null) return true;
        return _itemStack.CanAddItem(item);
    }

    /// <summary>
    /// <para>
    /// Set item stack for this slot, this will completely replace current item stack.
    /// If you want the item stack to be added to current stack, use the <see cref="AddItemStack"/>.
    /// </para>
    /// <para>为物品槽设置物品堆，将完全替换掉当前物品堆。如果想要物品堆叠加至该物品堆，请使用<see cref="AddItemStack"/></para>
    /// </summary>
    /// <returns>
    /// <para>The item stack that was previously in this slot</para>
    /// <para>该槽位中原本的物品堆</para>
    /// </returns>
    public IItemStack? ReplaceItemStack(IItemStack? newItemStack)
    {
        var result = _itemStack;
        _itemStack = newItemStack;

        UpdateAllDisplay();

        return result;
    }

    /// <summary>
    /// <para>Try to add an item to this slot, if it can't be added to this slot, return false</para>
    /// <para>尝试向当前槽位中加入物品，如果该物品不能被放入该槽位，返回false</para>
    /// </summary>
    public bool AddItem(IItem_New item)
    {
        bool result;
        if (_itemStack is null)
        {
            _itemStack = IItemStack.FromItem(item);
            result = true;
        }
        else
        {
            result = _itemStack.AddItem(item);
        }

        if (result)
        {
            UpdateAllDisplay();
        }

        return result;
    }

    /// <summary>
    /// <para>Try to combine an item stack into this slot</para>
    /// <para>尝试将一个物品堆合并至该槽位中</para>
    /// </summary>
    /// <returns>
    /// <para>If the source item stack is empty after the operation is completed</para>
    /// <para>操作完成后，源物品堆是否被取空</para>
    /// </returns>
    public bool AddItemStack(IItemStack itemStack)
    {
        bool result;
        if (_itemStack is null)
        {
            _itemStack = itemStack;
            result = false;
        }
        else
        {
            result = _itemStack.TakeFrom(itemStack);
        }

        UpdateAllDisplay();
        return result;
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
        if (_control == null) return;
        if (_itemStack == null)
        {
            _control.TooltipText = null;
            return;
        }

        if (Config.IsDebug())
        {
            var debugText = TranslationServerUtils.Translate("item_prompt_debug");
            if (debugText != null)
            {
                _control.TooltipText = string.Format(debugText, _itemStack.Id,
                                                     TranslationServerUtils.Translate(_itemStack.Name),
                                                     _itemStack.Quantity, _itemStack.MaxQuantity, _itemStack.GetType().Name,
                                                     TranslationServerUtils.Translate(_itemStack.Description));
            }
        }
        else
        {
            _control.TooltipText = TranslationServerUtils.Translate(_itemStack.Name) + "\n" +
                                   TranslationServerUtils.Translate(_itemStack.Description);
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

        switch (_itemStack?.Quantity)
        {
            case null or 1:
                _quantityLabel.Hide();
                return;
            default:
                //When the quantity is not null or 1, we display the quantity.
                //当数量不为null或1时，我们显示数量
                _quantityLabel.Text = _itemStack?.Quantity.ToString();
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
            _iconTextureRect.Texture = _itemStack?.Icon;
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
        _quantityLabel.Hide();
    }
}