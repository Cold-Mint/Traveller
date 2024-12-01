using System;
using System.Collections.Generic;
using ColdMint.scripts.debug;
using ColdMint.scripts.inventory;
using ColdMint.scripts.map.events;
using ColdMint.scripts.projectile;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.weapon;

/// <summary>
/// <para>Projectile weapons</para>
/// <para>抛射体武器</para>
/// </summary>
/// <remarks>
///<para>These weapons can fire projectiles to attack the enemy.For example: guns and scepters.Generate a bullet to attack the enemy.</para>
///<para>这类武器可发射抛射体，攻击敌人。例如：枪支和法杖。生成一个子弹攻击敌人。</para>
/// </remarks>
public partial class ProjectileWeapon : WeaponTemplate
{
    /// <summary>
    /// <para>The formation position of the projectile</para>
    /// <para>抛射体的生成位置</para>
    /// </summary>
    private Marker2D? _marker2D;

    /// <summary>
    /// <para>Number of slots for ranged weapons</para>
    /// <para>远程武器的槽位数量</para>
    /// </summary>
    [Export]
    private int _numberSlots;

    /// <summary>
    /// <para>SpellList</para>
    /// <para>法术列表</para>
    /// </summary>
    /// <remarks>
    ///<para>To make weapons out of the box, you need to configure pre-made spells here.</para>
    ///<para>为了使武器开箱即用，您需要在这里配置预制的法术。</para>
    /// </remarks>
    [Export]
    private string[]? _spellList;

    /// <summary>
    /// <para>How many projectiles are generated per fire</para>
    /// <para>每次开火生成多少个抛射体</para>
    /// </summary>
    public int NumberOfProjectiles { get; set; } = 1;

    private readonly List<ISpell> _spells = [];
    /// <summary>
    /// <para>Saves the position of a spell with projectile generation</para>
    /// <para>保存具有抛射体生成能力的法术位置</para>
    /// </summary>
    private readonly List<int> _spellProjectileIndexes = [];

    /// <summary>
    /// <para>safe Distance</para>
    /// <para>安全距离</para>
    /// </summary>
    /// <remarks>
    ///<para>When the distance from the enemy position to the weapon is less than or equal to the safe distance, then stop the fire. (The purpose of the safe distance is to prevent the projectile from firing at the player himself.)</para>
    ///<para>当敌人位置到武器的距离小于等于安全距离了，那么阻止发射。（设置安全距离的目的是为了防止抛射体对着玩家自身发射。）</para>
    /// </remarks>
    private float _safeDistance;

    /// <summary>
    /// <para>Whether to fire spells in sequence</para>
    /// <para>是否按顺序发射法术</para>
    /// </summary>
    [Export]
    private bool _fireSequentially;

    /// <summary>
    /// <para>The index used the last time a spell was cast</para>
    /// <para>上次发射法术时采用的索引</para>
    /// </summary>
    private int _lastUsedProjectileMagicIndex = -1;

    /// <summary>
    /// <para>Get next index</para>
    /// <para>获取下次索引</para>
    /// </summary>
    /// <returns></returns>
    private int GetNextProjectileMagicIndex()
    {
        if (_fireSequentially)
        {
            _lastUsedProjectileMagicIndex++;
            if (_lastUsedProjectileMagicIndex >= _spellProjectileIndexes.Count)
            {
                _lastUsedProjectileMagicIndex = 0;
            }
            return _lastUsedProjectileMagicIndex;
        }
        else
        {
            return RandomUtils.Instance.Next(0, _spellProjectileIndexes.Count);
        }
    }

    /// <summary>
    /// <para>Gets the loading range of the spell</para>
    /// <para>获取法术的加载范围</para>
    /// </summary>
    /// <returns>
    ///<para>Return array meaning: 0, starting position 1, ending position 2, with projectile generated spell position, length 3.</para>
    ///<para>返回数组的含义为：0,起始位置1,结束位置2,带有抛射体生成的法术位置，长度为3。</para>
    /// </returns>
    private int[] GetSpellScope()
    {
        var index = GetNextProjectileMagicIndex();
        var projectileSpellPosition = _spellProjectileIndexes[index];
        var endIndex = projectileSpellPosition;
        var startIndex = 0;
        if (index > 0)
        {
            //And the previous index can set the starting position.(The starting position is increased by 1 to avoid using spells with projectile generation as starting points.)
            //还有前面的索引可设定起始位置。(这里起始位置加1是为了避免 具有抛射体生成能力的法术 作为起点。)
            startIndex = _spellProjectileIndexes[index - 1] + 1;
        }
        if (index == _spellProjectileIndexes.Count - 1)
        {
            endIndex = _spells.Count - 1;
        }
        return [startIndex, endIndex, projectileSpellPosition];
    }

    public override int ItemType
    {
        get => Config.ItemType.ProjectileWeapon;
    }

    public override void LoadResource()
    {
        base.LoadResource();
        _marker2D = GetNode<Marker2D>("Marker2D");
        if (_marker2D != null)
        {
            _safeDistance = _marker2D.Position.LengthSquared();
        }
        if (SelfItemContainer == null)
        {
            SelfItemContainer = new UniversalItemContainer(_numberSlots);
            SelfItemContainer.AllowAddingItemByType(Config.ItemType.Spell);
            //Reload pre-made spells.
            //装填预制的法术。
            if (_spellList is not { Length: > 0 })
            {
                throw new ArgumentException(TranslationServerUtils.TranslateWithFormat("log_spell_list_is_null", Id ?? "null"));
            }
            var supportOutOfBox = false;
            foreach (var spellId in _spellList)
            {
                if (string.IsNullOrEmpty(spellId))
                {
                    continue;
                }
                var item = ItemTypeManager.CreateItem(spellId, this);
                if (item is not ISpell spell)
                {
                    continue;
                }
                //The spell is stored in memory and has not yet been loaded into the node tree. So we call the InvokeLoadResource method to initialize the resource.
                //法术保存在内存中，尚未加载到节点树。所以我们调用InvokeLoadResource方法来初始化资源。
                spell.LoadResource();
                if (!supportOutOfBox && spell.GetProjectile() != null)
                {
                    supportOutOfBox = true;
                }
                if (SelfItemContainer.CanAddItem(item))
                {
                    SelfItemContainer.AddItem(item);
                }
            }
            if (!supportOutOfBox)
            {
                //Long-range weapons must be used out of the box.
                //远程武器必须开箱即用
                throw new InvalidOperationException(TranslationServerUtils.TranslateWithFormat("log_weapon_must_be_used_out_of_the_box", Id ?? "null"));
            }
            UpdateSpellCache();
        }
    }

    /// <summary>
    /// <para>Update spell cache</para>
    /// <para>更新法术缓存</para>
    /// </summary>
    /// <remarks>
    ///<para>This will parse available spells from inside the item container.</para>
    ///<para>这将从物品容器内解析可用的法术。</para>
    /// </remarks>
    public void UpdateSpellCache()
    {
        if (SelfItemContainer == null)
        {
            return;
        }
        _spells.Clear();
        _spellProjectileIndexes.Clear();
        var totalCapacity = SelfItemContainer.GetTotalCapacity();
        for (var i = 0; i < totalCapacity; i++)
        {
            var item = SelfItemContainer.GetItem(i);
            if (item == null)
            {
                continue;
            }
            if (item is not ISpell spell)
            {
                continue;
            }
            _spells.Add(spell);
            var packedScene = spell.GetProjectile();
            if (packedScene != null)
            {
                //Has the ability to generate projectiles.
                //拥有抛射体生成能力。
                _spellProjectileIndexes.Add(_spells.Count - 1);
            }
        }
    }

    private void OnItemDataChangeEvent(ItemDataChangeEvent itemDataChangeEvent)
    {
        UpdateSpellCache();
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        if (SelfItemContainer != null)
        {
            SelfItemContainer.ItemDataChangeEvent += OnItemDataChangeEvent;
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (SelfItemContainer != null)
        {
            SelfItemContainer.ItemDataChangeEvent -= OnItemDataChangeEvent;
        }
    }

    protected override bool DoFire(Node2D? owner, Vector2 enemyGlobalPosition)
    {
        var enemyDistance = (enemyGlobalPosition - GlobalPosition).LengthSquared();
        if (enemyDistance <= _safeDistance)
        {
            LogCat.LogWarning("can_not_fire_from_safe_distance");
            return false;
        }
        if (owner == null)
        {
            LogCat.LogError("owner_is_null");
            return false;
        }

        if (_marker2D == null)
        {
            LogCat.LogError("marker2d_is_null");
            return false;
        }

        if (GameSceneDepend.ProjectileContainer == null)
        {
            LogCat.LogError("projectile_container_is_null");
            return false;
        }
        if (_spellProjectileIndexes.Count == 0)
        {
            LogCat.LogError("projectile_generate_magic_is_null");
            return false;
        }
        var spellScope = GetSpellScope();
        LogCat.LogWithFormat("projectile_weapon_range", LogCat.LogLabel.Default, true, string.Join(",", spellScope), _fireSequentially, _lastUsedProjectileMagicIndex);
        //The final spell is a projectile generator.
        //最后的法术是拥有抛射体生成能力的。
        var spellProjectile = _spells[spellScope[2]];
        var packedScene = spellProjectile.GetProjectile();
        if (packedScene == null)
        {
            LogCat.LogError("projectile_scene_is_null");
            return false;
        }
        ModifyWeapon(spellScope);
        for (var i = 0; i < NumberOfProjectiles; i++)
        {
            var projectile = NodeUtils.InstantiatePackedScene<Projectile>(packedScene);
            if (projectile == null)
            {
                LogCat.LogError("projectile_is_null");
                RestoreWeapon(spellScope);
                return false;
            }
            //Be sure to set the master node before modifying the projectile.
            //一定要在修改抛射体前设置主人节点。
            projectile.OwnerNode = OwnerNode;
            var velocity = _marker2D.GlobalPosition.DirectionTo(enemyGlobalPosition) * projectile.ActualSpeed;
            for (var s = spellScope[0]; s <= spellScope[1]; s++)
            {
                var spell = _spells[s];
                spell.ModifyProjectile(i, projectile, ref velocity);
            }
            NodeUtils.CallDeferredAddChild(GameSceneDepend.ProjectileContainer, projectile);
            projectile.LookAt(velocity);
            projectile.Velocity = velocity;
            projectile.Position = _marker2D.GlobalPosition;
        }
        RestoreWeapon(spellScope);
        return true;
    }

    /// <summary>
    /// <para>Modify weapon attributes</para>
    /// <para>修改武器属性</para>
    /// </summary>
    /// <param name="spellScope"></param>
    private void ModifyWeapon(int[] spellScope)
    {
        for (var i = spellScope[0]; i <= spellScope[1]; i++)
        {
            var spell = _spells[i];
            spell.ModifyWeapon(this);
        }
    }

    /// <summary>
    /// <para>Restores modifications to weapons</para>
    /// <para>恢复对武器的修改</para>
    /// </summary>
    /// <param name="spellScope"></param>
    private void RestoreWeapon(int[] spellScope)
    {
        for (var i = spellScope[0]; i <= spellScope[1]; i++)
        {
            var spell = _spells[i];
            spell.RestoreWeapon(this);
        }
    }
}