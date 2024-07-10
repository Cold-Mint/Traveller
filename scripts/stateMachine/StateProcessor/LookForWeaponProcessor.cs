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
    protected WeaponTemplate weaponTemplate;
    protected override void OnExecute(StateContext context, Node owner)
    {
        //Find weapons around your character.
        //查找角色周围的武器。
        if (owner is not AiCharacter aiCharacter)
        {
            LogCat.LogError("owner_is_not_AiCharacter");
            return;
        }

        if (GameSceneNodeHolder.WeaponContainer == null)
        {
            LogCat.LogError("weaponContainer_is_null");
            return;
        }

        NodeUtils.ForEachNode<WeaponTemplate>(GameSceneNodeHolder.WeaponContainer, template =>
        {
            if (template.GlobalPosition.DistanceTo(aiCharacter.GlobalPosition) > 100)
            {
                weaponTemplate = template;
                return true;
            }
            return false;
        });
    }

    public override State State => State.LookForWeapon;
}