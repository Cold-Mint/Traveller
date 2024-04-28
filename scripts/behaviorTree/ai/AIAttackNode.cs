using ColdMint.scripts.camp;
using ColdMint.scripts.character;
using Godot;

namespace ColdMint.scripts.behaviorTree.ai;

public class AIAttackNode : BehaviorTreeNodeTemplate
{
    public AICharacter Character { get; set; }

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
        CharacterTemplate closestEnemy = null;
        var closestDistance = float.MaxValue;
        var selfCamp = CampManager.GetCamp(Character.CampId);
        foreach (var node in nodesInTheAttackRange)
        {
            if (node is CharacterTemplate characterTemplate)
            {
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

                if (selfCamp.ID == characterCamp.ID)
                {
                    //如果是同一阵营，不攻击
                    continue;
                }

                var distance = characterTemplate.GlobalPosition - Character.GlobalPosition;
                var distanceLength = distance.Length();
                if (distanceLength < closestDistance)
                {
                    closestDistance = distanceLength;
                    closestEnemy = characterTemplate;
                }
            }
        }

        if (closestEnemy != null)
        {
            //There are the closest enemies
            //有距离最近的敌人
            var distance = closestEnemy.GlobalPosition - Character.GlobalPosition;
            Character.AttackObstacleDetection.TargetPosition = distance;
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