using ColdMint.scripts.behaviorTree.framework;
using ColdMint.scripts.character;
using ColdMint.scripts.weapon;

namespace ColdMint.scripts.behaviorTree.ai;

/// <summary>
/// <para>AI巡逻节点</para>
/// </summary>
public class AIPatrolNode : SelectorNode
{
    public AICharacter Character { get; set; }

    public override IBehaviorTreeNode SelectNode(bool isPhysicsProcess, double delta, IBehaviorTreeNode[] children)
    {
        if (Character.NodesInTheAttackRange.Length > 1)
        {
            if (Character.CurrentItem == null)
            {
                //No weapon
                //没有武器
                if (Character.NodesInTheAttackRange.Length > 0)
                {
                    var weaponTemplates = Character.GetCanPickedWeapon();
                    if (weaponTemplates.Length > 0)
                    {
                        var aiPickNode = GetChild<AIPickNode>(null);
                        if (aiPickNode != null)
                        {
                            return aiPickNode;
                        }
                    }
                }

                //No weapon, and no weapon to pick up, then try to escape
                //没有武器，且没有武器可捡，那么尝试逃跑
                var aiRotorNode = GetChild<AIRotorNode>(null);
                if (aiRotorNode != null)
                {
                    return aiRotorNode;
                }

                return children[0];
            }

            //There are enemies around
            //周围有敌人
            if (Character.AttackObstacleDetection.GetCollider() == null)
            {
                var aiAttackNode = GetChild<AIAttackNode>(null);
                if (aiAttackNode != null)
                {
                    return aiAttackNode;
                }
            }
        }

        if (Character.WallDetection.GetCollider() != null)
        {
            //Encounter a wall
            //遇到墙壁
            var aiRotorNode = GetChild<AIRotorNode>(null);
            if (aiRotorNode != null)
            {
                return aiRotorNode;
            }
        }

        return children[0];
    }
}