using System;
using System.Collections.Generic;
using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using ColdMint.scripts.utils;
using ColdMint.scripts.weapon;
using Godot;

namespace ColdMint.scripts.stateMachine.StateProcessor;

/// <summary>
/// <para>Weapon seeking condition</para>
/// <para>寻找武器状态</para>
/// </summary>
public class LookForWeaponProcessor : StateProcessorTemplate
{
    private WeaponTemplate? _targetWeapon;

    /// <summary>
    ///<para></para>
    ///<para>停止寻找武器的时间</para>
    /// </summary>
    /// <remarks>
    /// <para>If you don't find a weapon in a while, give up looking</para>
    /// <para>如果在一段时间内没找到武器，那么放弃寻找</para>
    /// </remarks>
    private DateTime _endTime;

    /// <summary>
    /// <para>When no weapon was found, how long did they give up looking</para>
    /// <para>当没有找到武器，多长时间放弃寻找</para>
    /// </summary>
    public TimeSpan RecoveryTimeSpan { get; set; } = TimeSpan.FromMilliseconds(150);

    public override void Enter(StateContext context)
    {
        UpdateEndTime();
    }

    /// <summary>
    /// <para>Update end time</para>
    /// <para>更新结束时间</para>
    /// </summary>
    private void UpdateEndTime()
    {
        _endTime = DateTime.Now + RecoveryTimeSpan;
    }

    protected override void OnExecute(StateContext context, Node owner)
    {
        if (owner is not AiCharacter aiCharacter)
        {
            LogCat.LogError("owner_is_not_AiCharacter", LogCat.LogLabel.LookForWeaponProcessor);
            return;
        }

        if (_targetWeapon == null)
        {
            if (DateTime.Now > _endTime)
            {
                //The search for the weapon ran out of time
                //寻找武器时超时
                LogCat.Log("search_for_weapon_timeout", LogCat.LogLabel.LookForWeaponProcessor);
                context.CurrentState = State.Flee;
                return;
            }
        }
        else
        {
            if (_targetWeapon.Picked)
            {
                //If the weapon we're looking for gets picked up, we find a new one.
                //如果我们要拿的武器被别人捡了，那么重新找新的武器。
                _targetWeapon = null;
                UpdateEndTime();
                return;
            }

            //If the nearest weapon is found, move the character to the weapon.
            //如果有最近的武器被找到了，那么将角色移动到武器旁边。
            var weaponTemplates = aiCharacter.GetCanPickedWeapon();
            //Weapons are not in the range of the pickup.
            //武器没在拾捡范围内。
            if (weaponTemplates.Length == 0)
            {
                LogCat.Log("weapon_not_in_pickup_range", LogCat.LogLabel.LookForWeaponProcessor);
                aiCharacter.SetTargetPosition(_targetWeapon.GlobalPosition);
            }
            else
            {
                var haveWeapon = false;
                foreach (var weaponTemplate in weaponTemplates)
                {
                    if (weaponTemplate == _targetWeapon)
                    {
                        haveWeapon = true;
                    }
                }

                if (haveWeapon)
                {
                    var pickResult = aiCharacter.PickItem(_targetWeapon);
                    if (pickResult)
                    {
                        context.CurrentState = context.PreviousState;
                        //Successfully picked up the weapon.
                        //成功拾起武器。
                        LogCat.Log("weapon_picked_up", LogCat.LogLabel.LookForWeaponProcessor);
                    }
                    else
                    {
                        _targetWeapon = null;
                        UpdateEndTime();
                        //Weapon failed to pick up.
                        //武器捡起时失败。
                        LogCat.Log("weapon_pickup_failed", LogCat.LogLabel.LookForWeaponProcessor);
                    }
                }
                else
                {
                    //No weapons are included in the pickup area.
                    //拾捡范围内不包含武器。
                    LogCat.Log("weapon_not_in_pickup_range", LogCat.LogLabel.LookForWeaponProcessor);
                    aiCharacter.SetTargetPosition(_targetWeapon.GlobalPosition);
                }
            }

            return;
        }

        if (aiCharacter.ScoutWeaponDetected())
        {
            //Weapons were found in the character's recon area.
            //在角色的侦察范围内发现了武器。
            //We search for the nearest weapon.
            //我们搜索最近的武器。
            LogCat.Log("weapon_detected", LogCat.LogLabel.LookForWeaponProcessor);
            var weaponTemplates = aiCharacter.GetWeaponInScoutArea();
            if (weaponTemplates == null || weaponTemplates.Length == 0)
            {
                //The weapon may have been lost or taken by someone else.
                //武器可能已丢失，或被他人占用。
                LogCat.Log("weapon_lost", LogCat.LogLabel.LookForWeaponProcessor);
                return;
            }

            var nodes = new List<Node>();
            foreach (var weapon in weaponTemplates)
            {
                if (weapon is Node newNode)
                {
                    nodes.Add(newNode);
                }
            }

            var node = NodeUtils.GetTheNearestNode(aiCharacter, nodes.ToArray());
            if (node == null)
            {
                //When looking for the nearest node, return null.
                //查找最近的节点时，返回null。
                LogCat.Log("nearest_node_is_null", LogCat.LogLabel.LookForWeaponProcessor);
                return;
            }

            if (node is WeaponTemplate weaponTemplate)
            {
                _targetWeapon = weaponTemplate;
            }
            else
            {
                LogCat.LogError("node_is_not_WeaponTemplate", LogCat.LogLabel.LookForWeaponProcessor);
            }
        }
        else
        {
            //No weapons detected
            //没有检测到武器
            LogCat.Log("no_weapon_detected", LogCat.LogLabel.LookForWeaponProcessor);
        }
    }

    public override State State => State.LookForWeapon;
}