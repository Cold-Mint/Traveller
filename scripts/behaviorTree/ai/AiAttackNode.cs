using ColdMint.scripts.camp;
using ColdMint.scripts.character;

namespace ColdMint.scripts.behaviorTree.ai;

/// <summary>
/// <para>AI attack node</para>
/// <para>AI的攻击节点</para>
/// </summary>
public class AiAttackNode : BehaviorTreeNodeTemplate
{
    public AiCharacter? Character { get; set; }

    public override int Execute(bool isPhysicsProcess, double delta)
    {
        if (Character == null)
        {
            return Config.BehaviorTreeResult.Failure;
        }

        var nodesInTheAttackRange = Character.NodesInTheAttackRange;
        if (nodesInTheAttackRange.Length == 0)
        {
            //No nodes are in range of the attack
            //没有节点在攻击范围内
            return Config.BehaviorTreeResult.Failure;
        }

        //Save the nearest enemy
        //保存最近的敌人
        CharacterTemplate? closestEnemy = null;
        var closestDistance = float.MaxValue;
        var selfCamp = CampManager.GetCamp(Character.CampId);
        foreach (var node in nodesInTheAttackRange)
        {
            if (node is not CharacterTemplate characterTemplate)
            {
                continue;
            }
            
            if (node == Character)
            {
                continue;
            }

            var characterCamp = CampManager.GetCamp(characterTemplate.CampId);
            var canCause = CampManager.CanCauseHarm(selfCamp, characterCamp);
            if (!canCause)
            {
                continue;
            }

            if (selfCamp == null || characterCamp == null)
            {
                continue;
            }
            
            if (selfCamp.Id == characterCamp.Id)
            {
                //If it is the same side, do not attack, if allowed friend damage, this code will prevent the AI from actively attacking the player.
                //如果是同一阵营，不攻击，如果允许友伤，这段代码会阻止AI主动攻击玩家。
                continue;
            }

            var distance = characterTemplate.GlobalPosition.DistanceTo(Character.GlobalPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = characterTemplate;
            }
        }

        if (closestEnemy != null && Character.AttackObstacleDetection != null)
        {
            //With the nearest enemy and no obstacles
            //有距离最近的敌人，且没有障碍物
            var distanceVector2 = closestEnemy.GlobalPosition - Character.GlobalPosition;
            Character.AttackObstacleDetection.TargetPosition = distanceVector2;
            if (Character.AttackObstacleDetection.GetCollider() == null)
            {
                Character.StopMoving();
                Character.AimTheCurrentItemAtAPoint(closestEnemy.GlobalPosition);
                Character.UseItem(closestEnemy.GlobalPosition);
            }
        }

        return Config.BehaviorTreeResult.Success;
    }
}