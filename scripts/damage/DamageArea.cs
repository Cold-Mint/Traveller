using System.Collections.Generic;
using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.damage;

/// <summary>
/// <para>DamageArea</para>
/// <para>伤害区域</para>
/// </summary>
public partial class DamageArea : Area2D
{

    private int _maxDamage = 15;
    private readonly List<CharacterTemplate> _characterTemplates = new();

    public override void _Ready()
    {
        base._Ready();
        InputPickable = false;
        SetCollisionMaskValue(Config.LayerNumber.Player, true);
        SetCollisionMaskValue(Config.LayerNumber.Mob, true);
        Monitoring = true;
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    /// <summary>
    /// <para>When a player or creature enters the damage zone</para>
    /// <para>当玩家或生物进入伤害区域</para>
    /// </summary>
    /// <param name="body"></param>
    private void OnBodyExited(Node2D body)
    {
        LogCat.Log("BodyExited");
        if (body is CharacterTemplate characterTemplate)
        {
            _characterTemplates.Remove(characterTemplate);
        }
    }

    /// <summary>
    /// <para>When a player or creature exits the damage area</para>
    /// <para>当玩家或生物退出伤害区域</para>
    /// </summary>
    /// <param name="body"></param>
    private void OnBodyEntered(Node2D body)
    {
        LogCat.Log("BodyEntered");
        if (body is CharacterTemplate characterTemplate)
        {
            _characterTemplates.Add(characterTemplate);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        foreach (var characterTemplate in _characterTemplates)
        {
            var fixedDamage = new FixedDamage(5);
            characterTemplate.Damage(fixedDamage);
        }
    }
}