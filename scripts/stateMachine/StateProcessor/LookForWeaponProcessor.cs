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
    protected WeaponTemplate? TargetWeapon;

    protected override void OnExecute(StateContext context, Node owner)
    {
        if (owner is not AiCharacter aiCharacter)
        {
            LogCat.LogError("owner_is_not_AiCharacter", LogCat.LogLabel.LookForWeaponProcessor);
            return;
        }

        if (TargetWeapon != null)
        {
            //If the nearest weapon is found, move the character to the weapon.
            //如果有最近的武器被找到了，那么将角色移动到武器旁边。
            aiCharacter.SetTargetPosition(TargetWeapon.GlobalPosition);
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
                TargetWeapon = weaponTemplate;
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