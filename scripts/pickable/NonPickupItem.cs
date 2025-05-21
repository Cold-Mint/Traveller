using System;
using ColdMint.scripts.character;
using ColdMint.scripts.inventory;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.pickable;

/// <summary>
/// <para>NonPickupItem</para>
/// <para>不可捡起的物品</para>
/// </summary>
/// <remarks>
///<para>These items perform certain events when the player touches them.</para>
///<para>这类物品在玩家触碰到它们时执行某些事件。</para>
/// <para>For items that cannot be picked up, although the item interface is implemented, it cannot be picked up by the player.</para>
/// <para>对于不可拾起的物品来说，虽然实现了物品接口，但是其不能被玩家捡起。</para>
/// </remarks>
public partial class NonPickupItem : RigidBody2D, IItem
{
    /// <summary>
    /// <para>EntityCollisionMode</para>
    /// <para>实体碰撞模式</para>
    /// </summary>
    [Export] private int _entityCollisionMode = Config.EntityCollisionMode.None; //skipcq:CS-R1137

    [Export] private string? _itemName; //skipcq:CS-R1137

    public override void _Ready()
    {
        base._Ready();
        InputPickable = true;
        SetCollisionMaskValue(Config.LayerNumber.Wall, true);
        SetCollisionMaskValue(Config.LayerNumber.Platform, true);
        SetCollisionMaskValue(Config.LayerNumber.Floor, true);
        SetCollisionMaskValue(Config.LayerNumber.Barrier, true);
        if (_entityCollisionMode == Config.EntityCollisionMode.OnlyPlayers)
        {
            SetCollisionMaskValue(Config.LayerNumber.Player, true);
        }
        else if (_entityCollisionMode == Config.EntityCollisionMode.PlayersAndEntity)
        {
            SetCollisionMaskValue(Config.LayerNumber.Player, true);
            SetCollisionMaskValue(Config.LayerNumber.Mob, true);
        }
    }

    public override void _MouseEnter()
    {
        if (string.IsNullOrEmpty(_itemName))
        {
            return;
        }

        var translation = TranslationServerUtils.Translate(_itemName);
        if (string.IsNullOrEmpty(translation))
        {
            return;
        }

        FloatLabelUtils.ShowFloatLabel(this, translation, Colors.White);
    }

    public override void _MouseExit()
    {
        FloatLabelUtils.HideFloatLabel();
    }

    public int EntityCollisionMode
    {
        get => _entityCollisionMode;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (_entityCollisionMode == Config.EntityCollisionMode.None)
        {
            return;
        }

        var collisionInfo = MoveAndCollide(Vector2.Zero, testOnly: true);
        if (collisionInfo == null)
        {
            return;
        }

        var node = (Node2D)collisionInfo.GetCollider();
        if (_entityCollisionMode == Config.EntityCollisionMode.OnlyPlayers)
        {
            if (node is Player player)
            {
                OnTouchPlayer(player);
            }
        }
        else if (_entityCollisionMode == Config.EntityCollisionMode.PlayersAndEntity)
        {
            if (node is Player player)
            {
                OnTouchPlayer(player);
            }
            else if (node is CharacterTemplate characterTemplate)
            {
                OnTouchCharacterTemplate(characterTemplate);
            }
        }
    }

    /// <summary>
    /// <para>When this pickable touches the player</para>
    /// <para>当此可拾捡物碰到玩家时</para>
    /// </summary>
    /// <param name="player"></param>
    protected virtual void OnTouchPlayer(Player player)
    {
    }

    /// <summary>
    /// <para>When this pickable touches a character (a creature other than the player, such as a computer-controlled character)</para>
    /// <para>当此可拾捡物碰到角色时（除了玩家以外的生物，例如由电脑控制的角色）</para>
    /// </summary>
    /// <param name="player"></param>
    protected virtual void OnTouchCharacterTemplate(CharacterTemplate player)
    {
    }

    public int Index { get; set; }
    public string? Id { get; set; }

    public void ShowSelf()
    {
    }

    public void QueueFreeSelf()
    {
    }

    public void HideSelf()
    {
    }

    public Texture2D? Icon { get; }
    public string? ItemName { get; }
    public string? Description { get; }
    public int Quantity { get; set; }
    public int MaxQuantity { get; }
    public int ItemType { get; }
    public bool IsSelect { get; set; }
    public IItemContainer? ItemContainer { get; set; }
    public IItemContainer? SelfItemContainer { get; set; }

    public int MergeableItemCount(IItem other, int unallocatedQuantity)
    {
        return 0;
    }

    public IItem? CreateItem(int number)
    {
        return null;
    }

    public bool Use(Node2D? owner, Vector2 targetGlobalPosition)
    {
        return false;
    }

    public Action<Node2D, Vector2>? OnThrow { get; set; }
    public Action<CharacterTemplate>? OnPickUp { get; set; }
}