using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>HotBar</para>
/// <para>快捷物品栏</para>
/// </summary>
public partial class HotBar : HBoxContainer
{
    private IItemContainer? _itemContainer;
    private IItemContainerDisplay? _itemContainerDisplay;

    public override void _Ready()
    {
        base._Ready();
        var universalItemContainer = new UniversalItemContainer(Config.HotBarSize);
        _itemContainer = universalItemContainer;
        universalItemContainer.AllowItemTypesExceptPlaceholder();
        _itemContainer.SupportSelect = true;
        _itemContainerDisplay = new ItemSlotContainerDisplay(this);
        _itemContainerDisplay.BindItemContainer(_itemContainer);
        NodeUtils.DeleteAllChild(this);
    }


    public override void _Process(double delta)
    {
        base._Process(delta);
        if (GameSceneDepend.Player == null)
        {
            return;
        }

        if (GameSceneDepend.Player.Camera2D == null)
        {
            return;
        }

        if (GameSceneDepend.Player.Camera2D.FreeVision)
        {
            return;
        }

        if (Input.IsActionJustPressed("hotbar_next"))
        {
            //Mouse wheel down
            //鼠标滚轮向下
            _itemContainer?.SelectNextItem();
        }

        if (Input.IsActionJustPressed("hotbar_previous"))
        {
            //Mouse wheel up
            //鼠标滚轮向上
            _itemContainer?.SelectPreviousItem();
        }

        if (Input.IsActionJustPressed("hotbar_1"))
        {
            SelectItemSlotByHotBarShortcutKey(0);
        }

        if (Input.IsActionJustPressed("hotbar_2"))
        {
            SelectItemSlotByHotBarShortcutKey(1);
        }

        if (Input.IsActionJustPressed("hotbar_3"))
        {
            SelectItemSlotByHotBarShortcutKey(2);
        }

        if (Input.IsActionJustPressed("hotbar_4"))
        {
            SelectItemSlotByHotBarShortcutKey(3);
        }

        if (Input.IsActionJustPressed("hotbar_5"))
        {
            SelectItemSlotByHotBarShortcutKey(4);
        }

        if (Input.IsActionJustPressed("hotbar_6"))
        {
            SelectItemSlotByHotBarShortcutKey(5);
        }

        if (Input.IsActionJustPressed("hotbar_7"))
        {
            SelectItemSlotByHotBarShortcutKey(6);
        }

        if (Input.IsActionJustPressed("hotbar_8"))
        {
            SelectItemSlotByHotBarShortcutKey(7);
        }

        if (Input.IsActionJustPressed("hotbar_9"))
        {
            SelectItemSlotByHotBarShortcutKey(8);
        }
    }

    /// <summary>
    /// <para>Select the HotBar project using the shortcut keys</para>
    /// <para>通过快捷键选择HotBar项目</para>
    /// </summary>
    /// <para>The Pc version of the shortcut key index is 0-9</para>
    /// <para>Pc版本的快捷键索引为0-9</para>
    /// <param name="shortcutKeyIndex"></param>
    private void SelectItemSlotByHotBarShortcutKey(int shortcutKeyIndex)
    {
        if (_itemContainer == null)
        {
            return;
        }

        _itemContainer.SelectItem(shortcutKeyIndex);
    }

    public IItemContainer? GetItemContainer()
    {
        return _itemContainer;
    }
}