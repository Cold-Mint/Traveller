using System;

using ColdMint.scripts.character;
using ColdMint.scripts.pickable;
using ColdMint.scripts.damage;

using Godot;

namespace ColdMint.scripts.item.weapon;

/// <summary>
/// <para>WeaponTemplate</para>
/// <para>武器模板</para>
/// </summary>
public abstract partial class WeaponTemplate : PickAbleTemplate
{
    private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void Use(Node2D? owner, Vector2 targetGlobalPosition)
    {
        Fire(owner, targetGlobalPosition);
    }


    private DateTime? _lastFiringTime;

    /// <summary>
    /// <para>Firing interval</para>
    /// <para>开火间隔</para>
    /// </summary>
    private TimeSpan _firingInterval;
    private long _firingIntervalAsMillisecond = 100;
    [Export] protected long FiringIntervalAsMillisecond
    {
        get => _firingIntervalAsMillisecond;
        set
        {
            _firingIntervalAsMillisecond = value;
            _firingInterval = TimeSpan.FromMilliseconds(_firingIntervalAsMillisecond);
        }
    }


    /// <summary>
    /// <para>The recoil of the weapon</para>
    /// <para>武器的后坐力</para>
    /// </summary>
    /// <remarks>
    ///<para>When the weapon is fired, how much recoil is applied to the user, in units: the number of cells, and the X direction of the force is automatically inferred.</para>
    ///<para>武器开火，要对使用者施加多大的后坐力，单位：格数，力的X方向是自动推断的。</para>
    /// </remarks>
    [Export] private Vector2 _recoil;

    public override void _Ready() { }

    /// <summary>
    /// <para>Discharge of the weapon</para>
    /// <para>武器开火</para>
    /// </summary>
    /// <remarks>
    ///<param name="owner">
    ///<para>owner</para>
    ///<para>武器所有者</para>
    /// </param>
    /// <param name="enemyGlobalPosition">
    ///<para>enemyGlobalPosition</para>
    ///<para>敌人所在位置</para>
    /// </param>
    /// </remarks>
    public void Fire(Node2D? owner, Vector2 enemyGlobalPosition)
    {
        var nowTime = DateTime.Now;
        //If the present time minus the time of the last fire is less than the interval between fires, it means that the fire cannot be fired yet.
        //如果现在时间减去上次开火时间小于开火间隔，说明还不能开火。
        if (_lastFiringTime != null && nowTime - _lastFiringTime < _firingInterval)
        {
            return;
        }

        if (owner is CharacterTemplate characterTemplate)
        {
            //We check the recoil of the weapon before each firing.
            //我们在每次开火之前，检查武器的后坐力。
            if (_recoil != Vector2.Zero)
            {
                var force = new Vector2();
                var forceX = Math.Abs(_recoil.X);
                if (Math.Abs(RotationDegrees) < 90)
                {
                    //The weapon goes to the right and we apply a recoil to the left
                    //武器朝向右边我们向左施加后坐力
                    forceX = -forceX;
                }

                force.X = forceX * Config.CellSize;
                force.Y = _recoil.Y * Config.CellSize;
                characterTemplate.AddForce(force);
            }
        }

        DoFire(owner, enemyGlobalPosition);
        _lastFiringTime = nowTime;
    }

    /// <summary>
    /// <para>Execute fire</para>
    /// <para>执行开火</para>
    /// </summary>
    protected abstract void DoFire(Node2D? owner, Vector2 enemyGlobalPosition);
}