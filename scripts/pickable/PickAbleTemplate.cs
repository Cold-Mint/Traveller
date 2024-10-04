using System;
using ColdMint.scripts.camp;
using ColdMint.scripts.character;
using ColdMint.scripts.damage;
using ColdMint.scripts.debug;
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
    public virtual string Id { get; set; } = "ID";
    [Export] protected Texture2D? UniqueIcon { get; set; }

    public void ShowSelf()
    {
        Show();
    }

    public void QueueFreeSelf()
    {
        QueueFree();
    }

    public void HideSelf()
    {
        Hide();
    }

    public Texture2D Icon => UniqueIcon ?? ItemTypeManager.DefaultIconOf(Id);

    public new string Name
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

    /// <summary>
    /// <para>Enabled contact injury</para>
    /// <para>启用接触伤害</para>
    /// </summary>
    public bool EnableContactInjury;

    [Export] private int _minContactInjury = 1;
    [Export] private int _maxContactInjury = 2;

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
    /// <para>The number of tile maps that come into contact with this item</para>
    /// <para>与此物品接触的瓦片地图数量</para>
    /// </summary>
    private int _tileMapNumber;

    /// <summary>
    /// <para>This area represents the collision range of the weapon, and when other nodes enter this area, they will deal damage.</para>
    /// <para>这个区域表示武器的碰撞范围，当其他节点进入此区域时，会造成伤害。</para>
    /// </summary>
    private Area2D? _damageArea2D;

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

    public virtual void Use(Node2D? owner, Vector2 targetGlobalPosition)
    {
    }

    public virtual void OnThrow(Vector2 velocity)
    {

    }

    public override void _Ready()
    {
        _damageArea2D = GetNode<Area2D>("DamageArea2D");
        _damageArea2D.BodyEntered += OnBodyEnter;
        _damageArea2D.BodyExited += OnBodyExited;
        _tipLabel = GetNodeOrNull<Label>("TipLabel");
        InputPickable = true;
        SetCollisionMaskValue(Config.LayerNumber.Wall, true);
        SetCollisionMaskValue(Config.LayerNumber.Platform, true);
        SetCollisionMaskValue(Config.LayerNumber.Floor, true);
    }

    private void OnBodyExited(Node node)
    {
        if (Picked)
        {
            return;
        }

        //If it leaves the ground or walls.
        //如果离开了地面或墙壁。
        if (node is TileMapLayer)
        {
            _tileMapNumber--;
            if (_tileMapNumber == 0)
            {
                //No longer in contact with any shingles can cause injury
                //不再与任何瓦片接触后，可以造成伤害
                EnableContactInjury = true;
                LogCat.Log("after_no_longer_in_contact_with_any_tiles", LogCat.LogLabel.ContactInjury);
                SetCollisionMaskValue(Config.LayerNumber.Player, false);
            }
        }
    }

    /// <summary>
    /// <para>Use objects to smash enemies</para>
    /// <para>使用物品砸敌人</para>
    /// </summary>
    /// <param name="node"></param>
    private void OnBodyEnter(Node node)
    {
        if (Picked)
        {
            return;
        }

        if (node is TileMapLayer)
        {
            _tileMapNumber++;
            EnableContactInjury = false;
            LogCat.Log("contact_with_tiles_disables_damage", LogCat.LogLabel.ContactInjury);
            //Items can be pushed by the player when they are on the ground
            //当物品在地面上时，可被玩家推动
            SetCollisionMaskValue(Config.LayerNumber.Player, true);
        }
        else if (node is CharacterTemplate characterTemplate)
        {
            if (!EnableContactInjury)
            {
                LogCat.LogWarning("contact_damage_disabled_during_collision", LogCat.LogLabel.ContactInjury);
                return;
            }


            if (Owner == null)
            {
                LogCat.LogWarning("item_has_no_owner", LogCat.LogLabel.PickAbleTemplate);
                return;
            }

            if (Owner is not CharacterTemplate ownerCharacterTemplate)
            {
                LogCat.LogWarning("owner_of_the_item_is_not_character", LogCat.LogLabel.PickAbleTemplate);
                return;
            }

            //Determine if your side can cause damage
            //判断所属的阵营是否可以造成伤害
            var canCauseHarm = CampManager.CanCauseHarm(CampManager.GetCamp(ownerCharacterTemplate.CampId),
                CampManager.GetCamp(characterTemplate.CampId));
            if (!canCauseHarm)
            {
                LogCat.Log("no_damage_between_camps", LogCat.LogLabel.PickAbleTemplate);
                return;
            }

            //If allowed to cause harm
            //如果允许造成伤害
            var damage = new Damage
            {
                MaxDamage = Math.Abs(_maxContactInjury),
                MinDamage = Math.Abs(_minContactInjury),
                Attacker = ownerCharacterTemplate
            };
            damage.CreateDamage();
            damage.MoveLeft = LinearVelocity.X < 0;
            damage.Type = Config.DamageType.Physical;
            characterTemplate.Damage(damage);
            //Reduce speed after hitting enemies.
            //击中敌人后减少速度。
            LinearVelocity *= 1 - Config.ThrownItemsHitEnemiesReduceSpeedByPercentage;
        }
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
    /// <para>Please copy node properties within this function</para>
    /// <para>请在此函数内复制节点属性</para>
    /// </summary>
    /// <param name="node"></param>
    public virtual void CopyAttributes(Node node)
    {
        if (node is not PickAbleTemplate pickAbleTemplate)
        {
            return;
        }

        pickAbleTemplate.Id = Id;
    }
}