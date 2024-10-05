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
    /// <para>How many projectiles are generated per fire</para>
    /// <para>每次开火生成多少个抛射体</para>
    /// </summary>
    public int NumberOfProjectiles { get; set; } = 1;

    private readonly List<ISpell> _spells = new();
    /// <summary>
    /// <para>Saves the position of a spell with projectile generation</para>
    /// <para>保存具有抛射体生成能力的法术位置</para>
    /// </summary>
    private readonly List<int> _spellProjectileIndexes = new();

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
    private int _lastUsedProjectileMagicIndex;

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
                return 0;
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
    /// <returns></returns>
    private int[] GetSpellScope()
    {
        var index = GetNextProjectileMagicIndex();
        var endIndex = _spellProjectileIndexes[index];
        if (index > 0)
        {
            //And the previous index can set the starting position.(The starting position is increased by 1 to avoid using spells with projectile generation as starting points.)
            //还有前面的索引可设定起始位置。(这里起始位置加1是为了避免 具有抛射体生成能力的法术 作为起点。)
            var startIndex = _spellProjectileIndexes[index - 1] + 1;
            return [startIndex, endIndex];
        }
        else
        {
            return
            [
                0, endIndex
            ];
        }
    }

    public override int ItemType
    {
        get => Config.ItemType.ProjectileWeapon;
    }

    public override void _Ready()
    {
        base._Ready();
        _marker2D = GetNode<Marker2D>("Marker2D");
        SelfItemContainer = new UniversalItemContainer(_numberSlots);
        SelfItemContainer.AllowAddingItemByType(Config.ItemType.Spell);
    }

    private void OnItemDataChangeEvent(ItemDataChangeEvent itemDataChangeEvent)
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
                _spellProjectileIndexes.Add(i);
            }
        }
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

    protected override void DoFire(Node2D? owner, Vector2 enemyGlobalPosition)
    {
        if (owner == null)
        {
            LogCat.LogError("owner_is_null");
            return;
        }

        if (_marker2D == null)
        {
            LogCat.LogError("marker2d_is_null");
            return;
        }

        if (GameSceneDepend.ProjectileContainer == null)
        {
            LogCat.LogError("projectile_container_is_null");
            return;
        }
        if (_spellProjectileIndexes.Count == 0)
        {
            LogCat.LogError("projectile_generate_magic_is_null");
            return;
        }
        var spellScope = GetSpellScope();
        //The final spell is a projectile generator.
        //最后的法术是拥有抛射体生成能力的。
        var spellProjectile = _spells[spellScope[1]];
        var packedScene = spellProjectile.GetProjectile();
        if (packedScene == null)
        {
            return;
        }
        for (var i = spellScope[0]; i <= spellScope[1]; i++)
        {
            var spell = _spells[i];
            spell.ModifyWeapon(this);
        }
        for (var i = 0; i < NumberOfProjectiles; i++)
        {
            var projectile = NodeUtils.InstantiatePackedScene<Projectile>(packedScene);
            if (projectile == null) return;
            for (var s = spellScope[0]; s <= spellScope[1]; s++)
            {
                var spell = _spells[s];
                spell.ModifyProjectile(projectile);
            }
            NodeUtils.CallDeferredAddChild(GameSceneDepend.ProjectileContainer, projectile);
            projectile.Owner = owner;
            projectile.TargetNode = GameSceneDepend.TemporaryTargetNode;
            projectile.Velocity =
                _marker2D.GlobalPosition.DirectionTo(enemyGlobalPosition) * projectile.Speed;
            projectile.Position = _marker2D.GlobalPosition;
        }
        for (var i = spellScope[0]; i <= spellScope[1]; i++)
        {
            var spell = _spells[i];
            spell.RestoreWeapon(this);
        }
    }
}