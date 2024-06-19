using ColdMint.scripts.map.events;
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

    public override void _Ready()
    {
        base._Ready();
        _itemContainer = new UniversalItemContainer();
        EventManager.PlayerInstanceChangeEvent += PlayerInstanceChangeEvent;
        NodeUtils.DeleteAllChild(this);
        for (var i = 0; i < Config.HotBarSize; i++)
        {
            _itemContainer.AddItemSlot(this);
        }
    }


    /// <summary>
    /// <para>When the player instance changes, we update the binding of the item container</para>
    /// <para>当玩家实例改变时，我们更新物品容器的绑定</para>
    /// </summary>
    /// <param name="playerInstanceChangeEvent"></param>
    private void PlayerInstanceChangeEvent(PlayerInstanceChangeEvent playerInstanceChangeEvent)
    {
        if (_itemContainer is UniversalItemContainer universalItemContainer)
        {
            universalItemContainer.CharacterTemplate = GameSceneNodeHolder.Player;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("hotbar_next"))
        {
            //Mouse wheel down
            //鼠标滚轮向下
            GameSceneNodeHolder.HideBackpackUiContainerIfVisible();
            _itemContainer?.SelectTheNextItemSlot();
        }

        if (Input.IsActionJustPressed("hotbar_previous"))
        {
            //Mouse wheel up
            //鼠标滚轮向上
            GameSceneNodeHolder.HideBackpackUiContainerIfVisible();
            _itemContainer?.SelectThePreviousItemSlot();
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
        GameSceneNodeHolder.HideBackpackUiContainerIfVisible();
        if (_itemContainer == null)
        {
            return;
        }
        _itemContainer.SelectItemSlot(shortcutKeyIndex);
    }

    public IItemContainer? GetItemContainer()
    {
        return _itemContainer;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventManager.PlayerInstanceChangeEvent -= PlayerInstanceChangeEvent;
    }
}