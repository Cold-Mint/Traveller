using System;
using ColdMint.scripts.inventory;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.pickable;

/// <summary>
/// <para>Templates for all fallen objects</para>
/// <para>所有掉落物的模板</para>
/// </summary>
public partial class PickAbleTemplate : RigidBody2D, IItem
{
    public int Index { get; set; }
    //Do not export this field because the ID is specified within yaml.
    //不要导出此字段，因为ID是在yaml内指定的。
    public virtual string? Id { get; set; }

    public void ShowSelf()
    {
        ProcessMode = ProcessModeEnum.Inherit;
        Show();
    }

    public void QueueFreeSelf()
    {
        QueueFree();
    }

    public void HideSelf()
    {
        ProcessMode = ProcessModeEnum.Disabled;
        Hide();
    }

    public Texture2D? Icon
    {
        get => ItemTypeManager.GetIconOf(Id);
    }
    public string ItemName
    {
        get
        {
            var key = $"item_{Id}";
            return TranslationServerUtils.Translate(key) ?? key;
        }
    }

    /// <summary>
    /// <para>Owner</para>
    /// <para>主人</para>
    /// </summary>
    public new Node2D? Owner { get; set; }

    public string Description
    {
        get
        {
            var key = $"item_{Id}_desc";
            return TranslationServerUtils.Translate(key) ?? key;
        }
    }

    public int Quantity { get; set; } = 1;

    /// <summary>
    /// <para>Whether the item is currently picked up</para>
    /// <para>当前物品是否被捡起了</para>
    /// </summary>
    public bool Picked { get; set; }

    public int MaxQuantity { get; set; } = 1;
    public virtual int ItemType
    {
        get => Config.ItemType.Unknown;
    }

    private bool _isSelected;

    public bool IsSelect
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }
            _isSelected = value;
            OnSelectChange(value);
        }
    }

    public IItemContainer? ItemContainer { get; set; }
    public IItemContainer? SelfItemContainer { get; set; }

    private Label? _tipLabel;

    /// <summary>
    /// <para></para>
    /// <para>当选中状态发生改变时</para>
    /// </summary>
    /// <param name="isSelected"></param>
    protected virtual void OnSelectChange(bool isSelected)
    {

    }

    public IItem? CreateItem(int number)
    {
        if (number == 0)
        {
            return null;
        }

        var duplicate = Duplicate();
        if (duplicate is PickAbleTemplate pickAbleTemplate)
        {
            pickAbleTemplate.CopyAttributes(this);
        }

        if (duplicate is not Node2D newNode2D)
        {
            return null;
        }

        newNode2D.GlobalPosition = GlobalPosition;
        if (duplicate is not IItem newItem)
        {
            duplicate.QueueFree();
            return null;
        }

        if (number < 0)
        {
            newItem.Quantity = Quantity;
        }
        else
        {
            newItem.Quantity = Math.Min(Quantity, number);
        }

        return newItem;
    }


    public int MergeableItemCount(IItem other, int unallocatedQuantity)
    {
        var freeQuantity = MaxQuantity - Quantity;
        if (freeQuantity == 0)
        {
            return 0;
        }

        if (other.Id != Id)
        {
            return 0;
        }

        return Math.Min(freeQuantity, unallocatedQuantity);
    }

    public virtual bool Use(Node2D? owner, Vector2 targetGlobalPosition)
    {
        return false;
    }

    public virtual void OnThrow(Vector2 velocity)
    {

    }

    private CollisionShape2D? _collisionShape2D;

    /// <summary>
    /// <para>Whether the resource has been loaded</para>
    /// <para>是否已加载过资源了</para>
    /// </summary>
    private bool _loadedResource;

    public override void _Ready()
    {
        LoadResource();
    }


    public virtual void LoadResource()
    {
        if (_loadedResource)
        {
            return;
        }
        _tipLabel = GetNodeOrNull<Label>("TipLabel");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        InputPickable = true;
        SetCollisionMaskValue(Config.LayerNumber.Wall, true);
        SetCollisionMaskValue(Config.LayerNumber.Platform, true);
        SetCollisionMaskValue(Config.LayerNumber.Floor, true);
        SetCollisionMaskValue(Config.LayerNumber.Barrier, true);
        _loadedResource = true;
    }

    public override void _MouseEnter()
    {
        if (Picked)
        {
            return;
        }

        if (_tipLabel == null)
        {
            return;
        }

        _tipLabel.Visible = true;
        _tipLabel.Text = Name;
        //Vertical Centering Tip
        //垂直居中提示
        var oldPosition = _tipLabel.Position;
        oldPosition.X = -_tipLabel.Size.X / 2;
        _tipLabel.Rotation = -Rotation;
        _tipLabel.Position = oldPosition;
    }

    public override void _MouseExit()
    {
        if (_tipLabel == null)
        {
            return;
        }

        _tipLabel.Visible = false;
    }

    /// <summary>
    /// <para>Flip item</para>
    /// <para>翻转物品</para>
    /// </summary>
    /// <param name="facingLeft"></param>
    public void Flip(bool facingLeft)
    {
    }

    /// <summary>
    /// <para>Collision forbidden shape</para>
    /// <para>禁用碰撞形状</para>
    /// </summary>
    /// <remarks>
    ///<para>Prevents pickables from blocking projectiles.</para>
    ///<para>防止可拾捡物阻挡抛射体。</para>
    /// </remarks>
    public void DisabledCollisionShape2D()
    {
        if (_collisionShape2D != null)
        {
            _collisionShape2D.Disabled = true;
        }
    }

    /// <summary>
    /// <para>EnabledCollisionShape2D</para>
    /// <para>启用碰撞形状</para>
    /// </summary>
    public void EnabledCollisionShape2D()
    {
        if (_collisionShape2D != null)
        {
            _collisionShape2D.Disabled = false;
        }
    }

    /// <summary>
    /// <para>Please copy node properties within this function</para>
    /// <para>请在此函数内复制节点属性</para>
    /// </summary>
    /// <param name="originalNode"></param>
    public void CopyAttributes(Node originalNode)
    {
        if (originalNode is not PickAbleTemplate originalPickAbleTemplate)
        {
            return;
        }
        Id = originalPickAbleTemplate.Id;
        SelfItemContainer = originalPickAbleTemplate.SelfItemContainer;
    }
}