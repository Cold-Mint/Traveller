using System;

using ColdMint.scripts.camp;
using ColdMint.scripts.character;
using ColdMint.scripts.damage;
using ColdMint.scripts.item;

using Godot;

namespace ColdMint.scripts.pickable;

/// <summary>
/// <para>Templates for all fallen objects</para>
/// <para>所有掉落物的模板</para>
/// </summary>
public partial class PickAbleTemplate : RigidBody2D, IItem
{
    [Export] public virtual string Id { get; set; } = "ID";
    [Export] protected Texture2D? UniqueIcon { get; set; }
    public Texture2D Icon => UniqueIcon ?? ItemTypeManager.DefaultIconOf(Id);
    [Export] protected string? UniqueName { get; set; }
    public new string Name => UniqueName ?? ItemTypeManager.DefaultNameOf(Id);
    [Export] protected string? UniqueDescription { get; set; }

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

    public string? Description => UniqueDescription ?? ItemTypeManager.DefaultDescriptionOf(Id);

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

    public virtual void Use(Node2D? owner, Vector2 targetGlobalPosition) { }

    public override void _Ready()
    {
        _damageArea2D = GetNode<Area2D>("DamageArea2D");
        _damageArea2D.BodyEntered += OnBodyEnter;
        _damageArea2D.BodyExited += OnBodyExited;
    }

    private void OnBodyExited(Node node)
    {
        if (Picked)
        {
            return;
        }

        //If it leaves the ground or walls.
        //如果离开了地面或墙壁。
        if (node is TileMap)
        {
            _tileMapNumber--;
            if (_tileMapNumber == 0)
            {
                //No longer in contact with any shingles can cause injury
                //不再与任何瓦片接触后，可以造成伤害
                EnableContactInjury = true;
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

        if (node is TileMap)
        {
            _tileMapNumber++;
            EnableContactInjury = false;
            //Items can be pushed by the player when they are on the ground
            //当物品在地面上时，可被玩家推动
            SetCollisionMaskValue(Config.LayerNumber.Player, true);
        }
        else if (node is CharacterTemplate characterTemplate)
        {
            if (!EnableContactInjury)
            {
                return;
            }


            if (Owner is not CharacterTemplate ownerCharacterTemplate)
            {
                return;
            }

            //Determine if your side can cause damage
            //判断所属的阵营是否可以造成伤害
            var canCauseHarm = CampManager.CanCauseHarm(CampManager.GetCamp(ownerCharacterTemplate.CampId),
                                                        CampManager.GetCamp(characterTemplate.CampId));
            if (!canCauseHarm)
            {
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


    /// <summary>
    /// <para>Flip item</para>
    /// <para>翻转物品</para>
    /// </summary>
    /// <param name="facingLeft"></param>
    public void Flip(bool facingLeft) { }

    public virtual void Destroy()
    {
        QueueFree();
    }

    public bool CanStackWith(IItem item) => false;
}