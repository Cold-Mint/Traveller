using ColdMint.scripts.character;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>HotBar</para>
/// <para>快捷物品栏</para>
/// </summary>
public partial class HotBar : HBoxContainer
{
    private UniversalItemContainer? _universalItemContainer;

    public override void _Ready()
    {
        base._Ready();
        _universalItemContainer = new UniversalItemContainer
        {
            CharacterTemplate = new Player()
        };
        NodeUtils.DeleteAllChild(this);
        for (var i = 0; i < Config.HotBarSize; i++)
        {
            _universalItemContainer.AddItemSlot(this, i);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("hotbar_next"))
        {
            //Mouse wheel down
            //鼠标滚轮向下
            _universalItemContainer?.SelectTheNextItemSlot();
        }

        if (Input.IsActionJustPressed("hotbar_previous"))
        {
            //Mouse wheel up
            //鼠标滚轮向上
            _universalItemContainer?.SelectThePreviousItemSlot();
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
        if (_universalItemContainer == null)
        {
            return;
        }
        _universalItemContainer.SelectItemSlot(shortcutKeyIndex);
    }

    public IItemContainer? GetItemContainer()
    {
        return _universalItemContainer;
    }
}