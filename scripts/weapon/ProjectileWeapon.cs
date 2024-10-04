using System.Collections.Generic;
using ColdMint.scripts.debug;
using ColdMint.scripts.inventory;
using ColdMint.scripts.map.events;
using ColdMint.scripts.projectile;
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
    [Export] public int NumberSlots { get; set; }

    private readonly List<IMagic> _magics = new();

    public override int ItemType
    {
        get => Config.ItemType.ProjectileWeapon;
    }

    public override void _Ready()
    {
        base._Ready();
        _marker2D = GetNode<Marker2D>("Marker2D");
        SelfItemContainer = new UniversalItemContainer(NumberSlots);
        SelfItemContainer.AllowAddingItemByType(Config.ItemType.Magic);
        SelfItemContainer.ItemDataChangeEvent += OnItemDataChangeEvent;
    }

    private void OnItemDataChangeEvent(ItemDataChangeEvent itemDataChangeEvent)
    {
        if (SelfItemContainer == null)
        {
            return;
        }
        _magics.Clear();
        var totalCapacity = SelfItemContainer.GetTotalCapacity();
        for (var i = 0; i < totalCapacity; i++)
        {
            var item = SelfItemContainer.GetItem(i);
            if (item == null)
            {
                continue;
            }
            if (item is not IMagic magic)
            {
                continue;
            }
            _magics.Add(magic);
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
        if (_magics.Count == 0)
        {
            LogCat.LogError("magics_is_null");
            return;
        }
    }
}