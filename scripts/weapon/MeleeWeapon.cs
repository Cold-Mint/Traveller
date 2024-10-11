using System.Collections.Generic;
using ColdMint.scripts.character;
using ColdMint.scripts.damage;
using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.weapon;

/// <summary>
/// <para>MeleeWeapon</para>
/// <para>近战武器</para>
/// </summary>
public partial class MeleeWeapon : WeaponTemplate
{

    /// <summary>
    /// <para>The damage area of the weapon</para>
    /// <para>武器的伤害区域</para>
    /// </summary>
    private Area2D? _weaponDamageArea;

    [Export]
    private int _maxDamage = 1;
    [Export]
    private int _minDamage = 1;
    private DamageTemplate? _damageTemplate;

    private readonly List<CharacterTemplate> _characterTemplates =
    [
    ];

    public override void LoadResource()
    {
        base.LoadResource();
        _weaponDamageArea = GetNode<Area2D>("WeaponDamageArea");
        _weaponDamageArea.InputPickable = false;
        _weaponDamageArea.SetCollisionMaskValue(Config.LayerNumber.Player, true);
        _weaponDamageArea.SetCollisionMaskValue(Config.LayerNumber.Mob, true);
        _weaponDamageArea.Monitoring = true;
        _weaponDamageArea.BodyEntered += OnBodyEntered;
        _weaponDamageArea.BodyExited += OnBodyExited;
        _weaponDamageArea.AreaEntered += AreaEntered;
        _weaponDamageArea.AreaExited += AreaExited;
        _damageTemplate = new Damage();
        _damageTemplate.MaxDamage = _maxDamage;
        _damageTemplate.MinDamage = _minDamage;
        _damageTemplate.Type = Config.DamageType.Physical;
    }

    private void AreaExited(Node2D node2D)
    {
        LogCat.Log("AreaExited" + node2D.Name, LogCat.LogLabel.MeleeWeapon);

    }

    private void AreaEntered(Node2D node2D)
    {
        LogCat.Log("AreaEntered" + node2D.Name, LogCat.LogLabel.MeleeWeapon);

    }
    /// <summary>
    /// <para>OnBodyEntered</para>
    /// <para>当敌人进入攻击范围</para>
    /// </summary>
    private void OnBodyEntered(Node2D node2D)
    {
        LogCat.Log("OnBodyEntered" + node2D.Name, LogCat.LogLabel.MeleeWeapon);
        if (node2D is CharacterTemplate characterTemplate)
        {
            _characterTemplates.Add(characterTemplate);
        }
    }

    /// <summary>
    /// <para>When the enemy is out of range</para>
    /// <para>当敌人离开攻击范围</para>
    /// </summary>
    /// <param name="node"></param>
    private void OnBodyExited(Node node)
    {
        LogCat.Log("OnBodyExited" + node.Name, LogCat.LogLabel.MeleeWeapon);
        if (node is CharacterTemplate characterTemplate)
        {
            _characterTemplates.Remove(characterTemplate);
        }
    }

    protected override bool DoFire(Node2D? owner, Vector2 enemyGlobalPosition)
    {
        if (_weaponDamageArea == null)
        {
            return false;
        }
        if (_damageTemplate == null)
        {
            return false;
        }
        if (owner == null)
        {
            LogCat.LogError("owner_is_null", LogCat.LogLabel.MeleeWeapon);
            return false;
        }

        if (_characterTemplates.Count == 0)
        {
            return false;
        }
        foreach (var characterTemplate in _characterTemplates)
        {
            characterTemplate.Damage(_damageTemplate);
        }
        return true;
    }
}