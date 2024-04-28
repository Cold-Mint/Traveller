using ColdMint.scripts.character;
using ColdMint.scripts.weapon;
using Godot;

namespace ColdMint.scripts.behaviorTree.ai;

/// <summary>
/// <para>Deal with AI picking up items</para>
/// <para>处理AI拾起物品的行为</para>
/// </summary>
public class AIPickNode : BehaviorTreeNodeTemplate
{


    public AICharacter Character { get; set; }

    public override int Execute(bool isPhysicsProcess, double delta)
    {
        if (Character == null)
        {
            return Config.BehaviorTreeResult.Failure;
        }

        if (Character.CurrentItem != null)
        {
            //If the character already has the item, we don't pick it up
            //如果角色已经持有物品了，我们就不再拾取
            return Config.BehaviorTreeResult.Success;
        }

        //Find the nearest item
        //查找距离最近的物品
        var childCount = Character.PickingRangeBodies.Length;
        if (childCount == 0)
        {
            //We can't pick things up without them
            //没有物品，我们不能捡起
            return Config.BehaviorTreeResult.Failure;
        }

        //The closest weapon
        //距离最近的武器
        WeaponTemplate closestWeapon = null;
        var closestDistance = float.MaxValue;
        foreach (var weaponTemplate in Character.GetCanPickedWeapon())
        {
            //If it's a weapon
            //如果是武器
            var distance = weaponTemplate.GlobalPosition - Character.GlobalPosition;
            var distanceLength = distance.Length();
            if (distanceLength < closestDistance)
            {
                closestDistance = distanceLength;
                closestWeapon = weaponTemplate;
            }   
        }

        //绘制一条线，从AI到武器
        // Draw a line from AI to weapon
        if (closestWeapon != null)
        {
            //If we find the nearest weapon
            //如果找到了最近的武器
            Character.PickItem(closestWeapon);
        }


        return Config.BehaviorTreeResult.Success;
    }
}