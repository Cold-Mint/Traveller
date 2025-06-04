using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColdMint.scripts.camp;
using ColdMint.scripts.damage;
using ColdMint.scripts.debug;
using ColdMint.scripts.heal;
using ColdMint.scripts.health;
using ColdMint.scripts.inventory;
using ColdMint.scripts.utils;
using ColdMint.scripts.loot;
using ColdMint.scripts.pickable;
using ColdMint.scripts.weapon;
using Godot;
using WeaponTemplate = ColdMint.scripts.weapon.WeaponTemplate;

namespace ColdMint.scripts.character;

/// <summary>
/// <para>CharacterTemplate</para>
/// <para>角色模板</para>
/// </summary>
/// <remarks>
///<para>Behavior shared by all characters</para>
///<para>所有角色共有的行为</para>
/// </remarks>
public partial class CharacterTemplate : CharacterBody2D
{
    // Get the gravity from the project settings to be synced with RigidBody nodes.
    // 从项目设置中获取与RigidBody节点同步的重力。
    protected float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    /// <summary>
    /// <para>How fast the character moves</para>
    /// <para>角色的移动速度</para>
    /// </summary>
    /// <remarks>
    ///<para>How many squares per second?</para>
    ///<para>每秒移动几格？</para>
    /// </remarks>
    protected const float Speed = 5f;

    /// <summary>
    /// <para>Speed multiplier</para>
    /// <para>速度乘数</para>
    /// </summary>
    protected float ProtectedSpeedScale = 1f;

    /// <summary>
    /// <para>Speed multiplier</para>
    /// <para>速度乘数</para>
    /// <remarks>
    ///<para>Set to 0.5 to move at 50% of the normal speed.</para>
    ///<para>设置为0.5则以正常速度的50%移动。</para>
    /// </remarks>
    /// </summary>
    public float SpeedScale
    {
        get => ProtectedSpeedScale;
        set => ProtectedSpeedScale = value > 1 ? 1 : value;
    }

    protected const float JumpVelocity = -240;
    public string? ReadOnlyCharacterName => TranslationServerUtils.Translate(_characterName);

    [Export] private string? _characterName;

    /// <summary>
    /// <para>Can mutate after death</para>
    /// <para>是否允许死后变异</para>
    /// </summary>
    [Export]
    public bool CanMutateAfterDeath { get; set; } = true;

    /// <summary>
    /// <para>Required For Wave Advance</para>
    /// <para>每波怪物必须的</para>
    /// </summary>
    /// <remarks>
    ///<para>If it is set to true, then killing the creature is necessary to move on to the next Wave (i.e. "Required For Wave Advance"); If set to false, the player can advance to the next wave without killing the creature.</para>
    ///<para>如果设置为true，那么杀死这个生物是进入下一波的必要条件（即"Required For Wave Advance"）；如果设置为false，则玩家不杀死这个生物也能进入下一波。</para>
    /// </remarks>
    [Export]
    public bool RequiredForWaveAdvance { get; set; } = true;

    protected IItemContainer? ProtectedItemContainer;

    //Item containers are used to store items.
    //物品容器用于存储物品。
    public IItemContainer? ItemContainer
    {
        get => ProtectedItemContainer;
        set
        {
            ProtectedItemContainer = value;
            WhenBindItemContainer(ProtectedItemContainer);
        }
    }


    //Items currently held
    //当前持有的物品
    private Node2D? _currentItem;

    public Node2D? CurrentItem
    {
        get => _currentItem;
        protected set
        {
            _currentItem = value;
            WhenUpdateCurrentItem(_currentItem);
        }
    }

    /// <summary>
    /// <para>When binding an item container to a character</para>
    /// <para>当为角色绑定物品容器时</para>
    /// </summary>
    /// <param name="itemContainer"></param>
    protected virtual void WhenBindItemContainer(IItemContainer? itemContainer)
    {
    }


    /// <summary>
    /// <para>When the items the character holds are updated</para>
    /// <para>当角色持有的物品更新时</para>
    /// </summary>
    /// <param name="currentItem">
    ///<para>Update finished items</para>
    ///<para>更新完成后的物品</para>
    /// </param>
    protected virtual void WhenUpdateCurrentItem(Node2D? currentItem)
    {
    }

    //Define a pickup range
    //定义一个拾起范围
    private Area2D? _pickingArea;

    private AnimatedSprite2D? _animatedSprite2D;

    //A marker that defines the location of the item
    //一个标记，定义物品的位置
    protected Marker2D? ItemMarker2D;

    //The original X-coordinate of the item marker
    //物品标记的原始X坐标
    private float _itemMarkerOriginalX;

    protected float ReadOnlyItemMarkerOriginalX => _itemMarkerOriginalX;

    //Face left?
    //面向左边吗
    public bool FacingLeft = false;

    //The force added by the AddForce method
    //由AddForce方法追加的力
    private Vector2 _additionalForce = Vector2.Zero;
    private bool _hasMouse;

    protected int CurrentHp;

    /// <summary>
    /// <para>Obtain biological health points in read-only form</para>
    /// <para>以只读的形式获取生物血量</para>
    /// </summary>
    public int Hp => CurrentHp;

    /// <summary>
    /// <para>Get the maximum health in read-only mode</para>
    /// <para>以只读形式获取最大血量</para>
    /// </summary>
    public int ReadOnlyMaxHp => MaxHp;

    //The initial health of the character after creation
    //角色创建后的初始血量
    [Export] private int _initialHp;

    [Export] protected int MaxHp;

    /// <summary>
    /// <para>The camp ID of the role</para>
    /// <para>角色的阵营ID</para>
    /// </summary>
    [Export] public string? CampId;

    private DamageNumberNodeSpawn? _damageNumber;

    /// <summary>
    /// <para>indestructible</para>
    /// <para>是否为无敌的</para>
    /// </summary>
    [Export] public bool Indestructible; // skipcq:CS-R1137

    [Export] private string? _lootId; // skipcq:CS-R1137

    private HealthBar? _healthBar;
    private DateTime _lastDamageTime;

    /// <summary>
    /// <para>Pick up all items within range</para>
    /// <para>拾捡范围内的所有物品</para>
    /// </summary>
    protected List<Node>? PickingRangeBodiesList;

    private AudioStreamPlayer2D? _audioStreamPlayer2D;

    [Export] protected AudioStream[]? DamageSounds;

    public Node[] PickingRangeBodies => PickingRangeBodiesList?.ToArray() ?? [];

    private ShaderMaterial? _outlineMaterial;

    /// <summary>
    /// <para>Full Hp Revive</para>
    /// <para>满血复活</para>
    /// </summary>
    public void FullHpRevive()
    {
        Revive(MaxHp);
    }

    /// <summary>
    /// <para>Resurrected character</para>
    /// <para>复活角色</para>
    /// </summary>
    /// <remarks>
    ///<para>Sets the amount of Hp a character has after resurrection</para>
    ///<para>设置角色复活后拥有的Hp</para>
    /// </remarks>
    public virtual void Revive(int newHp)
    {
        //If the new Hp is less than or equal to 0, there is no need to resurrect
        //如果新的Hp小于等于0，那么不需要复活
        if (newHp <= 0)
        {
            return;
        }

        if (CurrentHp > 0)
        {
            //If the current Hp is greater than 0, there is no need to revive
            //如果当前Hp大于0，那么不需要复活
            return;
        }

        //Check whether the new Hp is greater than the maximum Hp. If yes, set the current Hp to the maximum Hp. If no, set the current Hp to the new HP
        //判断新的Hp是否大于最大Hp，若大于那么将当前Hp设置为最大Hp，否则设置为新的Hp
        CurrentHp = newHp > MaxHp ? MaxHp : newHp;
        Show();
    }

    /// <summary>
    /// <para>Find the nearest item within the pickup area(Does not include items currently held)</para>
    /// <para>在拾捡范围内查找距离最近的物品（不包括当前持有的物品）</para>
    /// </summary>
    /// <returns></returns>
    public Node2D? FindTheNearestItem()
    {
        LogCat.Log("find_nearest_item");
        if (PickingRangeBodiesList == null || PickingRangeBodiesList.Count == 0)
        {
            return null;
        }

        return NodeUtils.GetTheNearestNode(this, PickingRangeBodiesList.ToArray(), true,
            node => !CanPickItem(node));
    }


    /// <summary>
    /// <para>Get all weapons within range of the pickup</para>
    /// <para>获取所有在拾捡范围内的武器</para>
    /// </summary>
    /// <returns></returns>
    public WeaponTemplate[] GetCanPickedWeapon()
    {
        var weaponTemplates = new List<WeaponTemplate>();
        foreach (var pickingRangeBody in PickingRangeBodies)
        {
            if (pickingRangeBody is not WeaponTemplate weaponTemplate) continue;
            if (weaponTemplate.Picked)
            {
                continue;
            }

            weaponTemplates.Add(weaponTemplate);
        }

        return weaponTemplates.ToArray();
    }

    public override void _Ready()
    {
        base._Ready();
        PickingRangeBodiesList = [];

        if (MaxHp <= 0)
        {
            MaxHp = Config.DefaultMaxHp;
        }

        if (_initialHp <= 0 || _initialHp > MaxHp)
        {
            //If the initial health is less than or equal to 0 or greater than the maximum health, then set it to the maximum health
            //如果初始血量小于等于0或者大于最大血量，那么将其设置为最大血量
            _initialHp = MaxHp;
        }

        CurrentHp = _initialHp;
        //The health bar of a creature may be null.
        //生物的健康条，可能为null。
        _healthBar = GetNodeOrNull<HealthBar>("HealthBar");
        if (_healthBar != null)
        {
            _healthBar.MaxValue = MaxHp;
        }

        ItemMarker2D = GetNodeOrNull<Marker2D>("ItemMarker2D");
        if (ItemMarker2D != null)
        {
            _itemMarkerOriginalX = ItemMarker2D.Position.X;
        }

        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _pickingArea = GetNodeOrNull<Area2D>("Area2DPickingArea");
        _damageNumber = GetNode<DamageNumberNodeSpawn>("DamageNumber");
        _audioStreamPlayer2D = GetNodeOrNull<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        if (_pickingArea != null)
        {
            //If true, the zone will detect objects or areas entering and leaving the zone.
            //如果为true，该区域将检测进出该区域的物体或区域。
            _pickingArea.Monitoring = true;
            //Other regions cannot detect our pick region
            //其他区域不能检测到我们的拾取区域
            _pickingArea.Monitorable = false;
            _pickingArea.BodyEntered += EnterThePickingRangeBody;
            _pickingArea.BodyExited += ExitThePickingRangeBody;
        }

        //The character cannot pass through the wall and floor
        //角色不能穿过墙壁和地板
        SetCollisionMaskValue(Config.LayerNumber.Wall, true);
        SetCollisionMaskValue(Config.LayerNumber.Floor, true);
        SetCollisionMaskValue(Config.LayerNumber.Barrier, true);
        InputPickable = true;

        // 初始化描边材质
        if (_animatedSprite2D != null)
        {
            var shader = ResourceLoader.Load<Shader>("res://shaders/outline.gdshader");
            if (shader != null)
            {
                _outlineMaterial = new ShaderMaterial
                {
                    Shader = shader
                };
            }
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (GameSceneDepend.TemporaryTargetNode == this)
        {
            GameSceneDepend.TemporaryTargetNode = null;
        }
    }

    /// <summary>
    /// <para>Play the specified sound effects</para>
    /// <para>播放指定的音效</para>
    /// </summary>
    /// <param name="audioStream"></param>
    protected void PlayAudioStream(AudioStream audioStream)
    {
        if (_audioStreamPlayer2D == null)
        {
            return;
        }

        if (_audioStreamPlayer2D.IsPlaying())
        {
            return;
        }

        _audioStreamPlayer2D.Stream = audioStream;
        _audioStreamPlayer2D.Play();
    }

    /// <summary>
    /// <para>PlayDamageAudio</para>
    /// <para>播放受伤音效</para>
    /// </summary>
    protected void PlayDamageAudio()
    {
        if (DamageSounds == null || DamageSounds.Length == 0)
        {
            return;
        }

        var randomIndex = GD.Randi() % DamageSounds.Length;
        PlayAudioStream(DamageSounds[randomIndex]);
    }


    public override void _MouseEnter()
    {
        _hasMouse = true;
        var canCauseHarm = false;
        if (GameSceneDepend.Player == null)
        {
            canCauseHarm = true;
        }
        else
        {
            var targetCamp = CampManager.GetCamp(CampId);
            var playerCamp = CampManager.GetCamp(GameSceneDepend.Player.CampId);
            if (CampManager.CanCauseHarm(targetCamp, playerCamp))
            {
                if (targetCamp != null && playerCamp != null)
                {
                    if (targetCamp.Id != playerCamp.Id)
                    {
                        //If the injured target is an enemy of the player, it is displayed as an enemy color
                        //如果受伤的目标是玩家的敌人，则显示为敌对颜色
                        canCauseHarm = true;
                    }
                }
            }
        }

        GameSceneDepend.TemporaryTargetNode = this;
        _outlineMaterial?.SetShaderParameter("color",
            canCauseHarm ? Config.ColorConfig.EnemyColor : Config.ColorConfig.FriendlyColor);
        if (_animatedSprite2D != null && _outlineMaterial != null)
        {
            _animatedSprite2D.Material = _outlineMaterial;
        }

        var translation = ReadOnlyCharacterName;
        if (!string.IsNullOrEmpty(translation))
        {
            FloatLabelUtils.ShowFloatLabel(this, translation,
                canCauseHarm ? Config.ColorConfig.EnemyColor : Config.ColorConfig.FriendlyColor);
        }
    }

    public override void _MouseExit()
    {
        _hasMouse = false;
        if (_animatedSprite2D != null)
        {
            _animatedSprite2D.Material = null;
        }

        FloatLabelUtils.HideFloatLabel();
    }

    /// <summary>
    /// <para>Whether you can pick up specified items</para>
    /// <para>是否能捡起指定物品</para>
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    protected bool CanPickItem(Node node)
    {
        if (_currentItem != null && node == _currentItem)
        {
            //Do not include your own belongings.
            //不包含自己手上的物品。
            return false;
        }

        if (node is PickAbleTemplate pickAbleTemplate)
        {
            //Does not contain items that have been picked up.
            //不包含已被捡起的物品。
            return !pickAbleTemplate.Picked;
        }

        return false;
    }

    /// <summary>
    /// <para>Pick up the specified items</para>
    /// <para>将指定物品拾起来</para>
    /// </summary>
    /// <param name="node2D"></param>
    /// <returns>
    ///<para>Whether successfully picked up</para>
    ///<para>是否成功拾起</para>
    /// </returns>
    public virtual bool PickItem(Node2D? node2D)
    {
        //Empty reference checking is implicitly performed here.
        //此处隐式的执行了空引用检查。
        if (node2D is not PickAbleTemplate pickAbleItem)
        {
            return false;
        }

        //The item store is marked null, or the item container is null.
        //物品存放的标记为null，或者物品容器为null。
        if (ItemMarker2D == null || ItemContainer == null)
        {
            return false;
        }

        //Check to see if you can fit the item into the container first.
        //先检查是否能将物品放入容器。
        var canAddItem = ItemContainer.CanAddItem(pickAbleItem);
        if (!canAddItem)
        {
            return false;
        }

        //Is it successfully added to the container?
        //再检查是否成功的添加到容器内了？
        var addSuccessNumber = ItemContainer.AddItem(pickAbleItem);
        if (addSuccessNumber <= 0)
        {
            return false;
        }

        //Set up routine handling of picked up items.
        //设置捡起物品的常规处理。
        //You can supplement picking up state handling for more types of objects here.
        //您可以在这里补充更多类型对象的捡起状态处理。
        pickAbleItem.ClearNewItemGlow();
        pickAbleItem.LoadResource();
        pickAbleItem.OwnerNode = this;
        pickAbleItem.Picked = true;
        pickAbleItem.Freeze = true;
        pickAbleItem.DisabledCollisionShape2D();
        pickAbleItem.OnPickUp?.Invoke(this);
        if (pickAbleItem is ProjectileWeapon projectileWeapon)
        {
            projectileWeapon.UpdateSpellCache();
        }

        if (_currentItem == null && ItemContainer.GetSelectItem() == pickAbleItem)
        {
            //If the selected item slot in the item container is a newly picked item, and there is no item in the hand, then we put the selected item into the hand.
            //如果物品容器内选中的物品槽是刚刚捡到的物品，且手里没有物品持有，那么我们将选中的物品放到手上。
            CurrentItem = pickAbleItem;
        }
        else
        {
            pickAbleItem.Hide();
            pickAbleItem.ProcessMode = ProcessModeEnum.Disabled;
        }

        NodeUtils.CallDeferredReparent(ItemMarker2D, pickAbleItem);
        return true;
    }


    /// <summary>
    /// <para>Use what you have in your hand</para>
    /// <para>使用手中的物品</para>
    /// </summary>
    public bool UseItem(Vector2 position)
    {
        if (_currentItem == null)
        {
            return false;
        }

        if (_currentItem is IItem item)
        {
            return item.Use(this, position);
        }

        return false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        //If the time difference between the last injury and the current time is greater than the time displayed in the health bar, the health bar is hidden
        //如果上次受到伤害的时间与当前时间的时间差大于健康条显示时间，则隐藏健康条
        if (_healthBar is { Visible: true })
        {
            var timeSpan = DateTime.UtcNow - _lastDamageTime;
            if (timeSpan > Config.HealthBarDisplaysTime)
            {
                _healthBar.Hide();
            }
        }
    }

    /// <summary>
    /// <para>Update the role's health bar</para>
    /// <para>更新角色的健康条</para>
    /// </summary>
    private void UpDataHealthBar()
    {
        if (_healthBar == null)
        {
            return;
        }

        var canCauseHarm = false;
        if (GameSceneDepend.Player == null)
        {
            //We didn't know who the player was, so we showed it as a hostile color
            //我们不知道玩家是谁，所以我们将其显示为敌对颜色
            _healthBar.SetEnemyTones();
            canCauseHarm = true;
        }
        else
        {
            //If we set up a player node, then compare the injured party ID to the player's party ID
            //如果我们设置了玩家节点，那么将受伤者的阵营ID与玩家的阵营ID进行比较
            var targetCamp = CampManager.GetCamp(CampId);
            var playerCamp = CampManager.GetCamp(GameSceneDepend.Player.CampId);
            if (CampManager.CanCauseHarm(targetCamp, playerCamp))
            {
                if (targetCamp != null && playerCamp != null)
                {
                    if (targetCamp.Id == playerCamp.Id)
                    {
                        //If an attack is allowed, and you are on the same side, it is displayed as a friendly color (friend damage).
                        //如果允许攻击，且属于同一阵营，则显示为友好颜色（友伤）
                        _healthBar.SetFriendlyTones();
                    }
                    else
                    {
                        //If the injured target is an enemy of the player, it is displayed as an enemy color
                        //如果受伤的目标是玩家的敌人，则显示为敌对颜色
                        _healthBar.SetEnemyTones();
                        canCauseHarm = true;
                    }
                }
            }
            else
            {
                _healthBar.SetFriendlyTones();
            }
        }

        var translation = ReadOnlyCharacterName;
        if (GameSceneDepend.ShowObjectDetails && _hasMouse && !string.IsNullOrEmpty(translation))
        {
            FloatLabelUtils.ShowFloatLabel(this, translation,
                canCauseHarm ? Config.ColorConfig.EnemyColor : Config.ColorConfig.FriendlyColor);
        }


        _healthBar.Show();
        _healthBar.Value = CurrentHp;
    }

    /// <summary>
    /// <para>Heal</para>
    /// <para>治疗角色</para>
    /// </summary>
    /// <remarks>
    ///<para>Note: When a character dies, healing does not directly bring them back to life. Call method <see cref="Revive"/> to revive the character before applying healing.</para>
    ///<para>注意：当角色死亡，治疗并不能使其直接复活。请调用<see cref="Revive"/>方法复活角色后再施加治疗。</para>
    /// </remarks>
    /// <param name="heal">
    ///<para>heal</para>
    ///<para>治疗量</para>
    /// </param>
    /// <returns></returns>
    public bool Heal(Heal heal)
    {
        if (CurrentHp <= 0)
        {
            return false;
        }

        var nextHp = Math.Min(CurrentHp + heal.HealAmount, MaxHp);
        var actualHealAmount = nextHp - CurrentHp;
        if (actualHealAmount == 0)
        {
            return false;
        }

        CurrentHp = nextHp;
        _damageNumber?.DisplayHeal(heal, actualHealAmount);
        UpDataHealthBar();
        OnHeal(heal);
        return true;
    }

    /// <summary>
    /// <para>Deal damage to the character</para>
    /// <para>对角色造成伤害</para>
    /// </summary>
    /// <param name="damage">
    ///<para>Damage template</para>
    ///<para>伤害模板</para>
    /// </param>
    /// <returns>
    ///<para>Return whether the character is dead</para>
    ///<para>返回本次伤害是否导致角色死亡。</para>
    /// </returns>
    public bool Damage(IDamage damage)
    {
        if (CurrentHp <= 0 || Indestructible)
        {
            return false;
        }

        _lastDamageTime = DateTime.UtcNow;
        _damageNumber?.DisplayDamage(damage);
        CurrentHp -= damage.Damage;
        OnHit(damage);
        if (CurrentHp <= 0)
        {
            //Character death
            //角色死亡
            OnDie(damage).GetAwaiter();
            return true;
        }

        PlayDamageAudio();
        UpDataHealthBar();
        return false;
    }

    /// <summary>
    /// <para>Create Loot Object</para>
    /// <para>创建战利品对象</para>
    /// </summary>
    private async Task CreateLootObject()
    {
        if (string.IsNullOrEmpty(_lootId))
        {
            return;
        }

        await LootListManager.GenerateLootObjectsAsync<Node2D>(GetParent(), LootListManager.GenerateLootData(_lootId),
            GlobalPosition);
    }


    /// <summary>
    /// <para>Add power to the character</para>
    /// <para>在角色身上添加力</para>
    /// </summary>
    /// <param name="force"></param>
    public void AddForce(Vector2 force)
    {
        _additionalForce += force;
    }

    protected virtual void OnHit(IDamage damage)
    {
    }

    protected virtual void OnHeal(Heal heal)
    {
    }

    /// <summary>
    /// <para>Handle the event of character death</para>
    /// <para>处理角色死亡的事件</para>
    /// </summary>
    /// <param name="damage"></param>
    protected virtual async Task OnDie(IDamage damage)
    {
        //If the attacker is not empty and the role name is not empty, then the role death message is printed
        //如果攻击者不为空，且角色名不为空，那么打印角色死亡信息
        var readOnlyName = ReadOnlyCharacterName;
        if (damage.Attacker != null && !string.IsNullOrEmpty(readOnlyName))
        {
            if (damage.Attacker is CharacterTemplate characterTemplate &&
                !string.IsNullOrEmpty(characterTemplate.ReadOnlyCharacterName))
            {
                LogCat.LogWithFormat("death_info", LogCat.LogLabel.Default, readOnlyName,
                    characterTemplate.ReadOnlyCharacterName);
            }
            else
            {
                LogCat.LogWithFormat("death_info", LogCat.LogLabel.Default, readOnlyName,
                    damage.Attacker.Name);
            }
        }

        if (_hasMouse)
        {
            if (_animatedSprite2D != null)
            {
                _animatedSprite2D.Material = null;
            }

            FloatLabelUtils.HideFloatLabel();
        }

        ThrowAllItemOnDie();
        await CreateLootObject();
        QueueFree();
    }

    /// <summary>
    /// <para>When an object enters the picking range</para>
    /// <para>当有物体进入拾捡范围时</para>
    /// </summary>
    /// <param name="node"></param>
    protected virtual void EnterThePickingRangeBody(Node node)
    {
        if (node is not IItem)
        {
            return;
        }

        LogCat.Log("enter_the_picking_range_body");
        PickingRangeBodiesList?.Add(node);
    }

    /// <summary>
    /// <para>When an object exit the picking range</para>
    /// <para>当有物体离开拾捡范围时</para>
    /// </summary>
    /// <param name="node"></param>
    protected virtual void ExitThePickingRangeBody(Node node)
    {
        if (node is not IItem)
        {
            return;
        }

        LogCat.Log("exit_the_picking_range_body");
        PickingRangeBodiesList?.Remove(node);
    }

    /// <summary>
    /// <para>Flip sprites or animations</para>
    /// <para>翻转精灵或动画</para>
    /// </summary>
    protected virtual void Flip()
    {
        if (_animatedSprite2D == null)
        {
            return;
        }

        _animatedSprite2D.FlipH = FacingLeft;
    }

    /// <summary>
    /// <para>Throw all items when the creature dies</para>
    /// <para>当生物死亡后抛出所有物品</para>
    /// </summary>
    private void ThrowAllItemOnDie()
    {
        //If the item container is null, then return
        //如果物品容器为null，那么返回
        if (ItemContainer == null)
        {
            return;
        }

        var len = ItemContainer.GetTotalCapacity();
        if (len == 0)
        {
            return;
        }

        CurrentItem = null;
        const float height = -Config.CellSize * 7;
        const float horizontalDirection = Config.CellSize * 15;
        for (var i = 0; i < len; i++)
        {
            //Generates a random number that controls the horizontal velocity of thrown items (range: 0.01 to 1)
            //生成一个随机数，用于控制抛出物品的水平方向速度(范围为：0.01到1)
            var percent = GD.Randf() + 0.01f;
            var horizontalVelocity = horizontalDirection * percent * GD.Randf() > 0.5 ? 1f : -1f;
            ThrowItem(i, -1, new Vector2(horizontalVelocity, height));
        }
    }

    /// <summary>
    /// <para>Throw item</para>
    /// <para>抛出物品</para>
    /// </summary>
    /// <param name="index">
    ///<para>Item slot index in item container</para>
    ///<para>物品容器内的物品槽位置</para>
    /// </param>
    /// <param name="number">
    /// <para>How many to throw</para>
    /// <para>要扔几个</para>
    /// <para>The amount passed into a negative number will throw all the items in the slot at once.</para>
    /// <para>数量传入一个负数，将一次扔出槽内的所有物品。</para>
    /// </param>
    /// <param name="velocity">
    ///<para>The speed to be applied to the item</para>
    ///<para>要施加到物品上的速度</para>
    /// </param>
    protected virtual bool ThrowItem(int index, int number, Vector2 velocity)
    {
        if (number == 0)
        {
            return false;
        }

        var item = ItemContainer?.GetItem(index);
        if (item is null) return false;
        //Less than 0, throw everything
        //小于0,扔出所有物品
        var actualQuantity = number < 0 ? item.Quantity : Math.Min(item.Quantity, number);
        for (var i = 0; i < actualQuantity; i++)
        {
            ThrowOneItem(index, item, velocity);
        }

        return true;
    }

    ///  <summary>
    ///  <para>Throw item</para>
    ///  <para>抛出物品</para>
    ///  </summary>
    ///  <param name="index"></param>
    ///  <param name="originalItem"></param>
    ///  <param name="velocity">
    /// <para>The speed to be applied to the item</para>
    /// <para>要施加到物品上的速度</para>
    ///  </param>
    private void ThrowOneItem(int index, IItem originalItem, Vector2 velocity)
    {
        //Remove the item from the item container
        //从物品容器内取出物品
        var item = originalItem.CreateItem(1);
        if (item is not Node2D node2D)
        {
            return;
        }

        if (this is not Player)
        {
            item.ApplyNewItemGlow();
        }

        originalItem.OnThrow?.Invoke(node2D, velocity);
        item.ItemContainer = null;
        NodeUtils.CallDeferredAddChild(NodeUtils.FindContainerNode(node2D, GetNode("/root")), node2D);
        if (item is PickAbleTemplate pickAbleTemplate)
        {
            if (GameSceneDepend.WeaponContainer == null)
            {
                return;
            }

            pickAbleTemplate.LoadResource();
            pickAbleTemplate.OwnerNode = this;
            pickAbleTemplate.Picked = false;
            //Setting an initial speed of 0 for items here prevents the problem of throwing items too fast.
            //在这里给物品设置一个为0的初始速度，可防止扔出物品时速度过快的问题。
            pickAbleTemplate.LinearVelocity = Vector2.Zero;
            pickAbleTemplate.EnabledCollisionShape2D();
            pickAbleTemplate.Freeze = false;
        }

        node2D.ProcessMode = ProcessModeEnum.Inherit;
        node2D.Show();
        //We apply force to objects.
        //我们给物品施加力。
        switch (node2D)
        {
            case CharacterBody2D characterBody2D:
                characterBody2D.Velocity = velocity;
                break;
            case RigidBody2D rigidBody2D:
                rigidBody2D.LinearVelocity = velocity;
                break;
            default:
                throw new InvalidOperationException("Unsupported node type encountered when applying force.");
        }

        ProtectedItemContainer?.RemoveItem(index, 1);
        originalItem.QueueFreeSelf();
    }


    public sealed override void _PhysicsProcess(double delta)
    {
        if (!Visible)
        {
            return;
        }

        //We continuously set the position of the items to prevent them from changing as we zoom in and out of the window.
        //我们持续设置物品的位置，为了防止放大缩小窗口时物品位置的变化。
        if (_currentItem != null)
        {
            _currentItem.Position = Vector2.Zero;
        }

        var velocity = Velocity;
        // The ref keyword passes its pointer to the following method so that it can be modified in the method.
        // ref关键字将其指针传递给下面的方法，以便在方法中修改它。
        HookPhysicsProcess(delta, ref velocity);
        // Add the gravity.
        //增加重力。
        if (!IsOnFloor())
            velocity.Y += Gravity * (float)delta;
        Velocity = velocity + _additionalForce;
        // 附加力随时间衰减
        // Decay the additional force over time
        _additionalForce = _additionalForce.Lerp(Vector2.Zero, 0.5f);
        MoveAndSlide();
    }

    /// <summary>
    /// <para>Aim the held item at a point</para>
    /// <para>使持有的物品瞄准某个点</para>
    /// </summary>
    public void AimTheCurrentItemAtAPoint(Vector2 position)
    {
        if (_currentItem == null)
        {
            //Do not currently hold any items.
            //当前没有持有任何物品。
            return;
        }

        // Apply the rotation Angle to the node
        // 将旋转角度应用于节点
        _currentItem.LookAt(position);
    }


    protected virtual void HookPhysicsProcess(double delta, ref Vector2 velocity)
    {
        //The cost of applying force in the X direction.
        //对X方向施加力消耗。
        if ((int)velocity.X == 0)
        {
            velocity.X = 0;
        }
        else
        {
            velocity.X *= 0.95f;
        }
    }
}