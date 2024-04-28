using ColdMint.scripts.behaviorTree.ai;
using ColdMint.scripts.behaviorTree.framework;
using ColdMint.scripts.character;

namespace ColdMint.scripts.behaviorTree.behavior;

/// <summary>
/// <para>Represents a behavior tree for patrol</para>
/// <para>表示巡逻的行为树</para>
/// </summary>
public class PatrolBehaviorTree : BehaviorTreeTemplate
{
    public AICharacter Character { get; set; }
    protected override IBehaviorTreeNode CreateRoot()
    {
        var patrolNode = new AIPatrolNode();
        var aiWalkNode = new AIWalkNode();
        var aiRotorNode = new AIRotorNode();
        var aIPickNode = new AIPickNode();
        var aiAttackNode = new AIAttackNode();
        aiWalkNode.Character = Character;
        patrolNode.Character = Character;
        aiRotorNode.Character = Character;
        aIPickNode.Character = Character;
        aiAttackNode.Character = Character;
        patrolNode.AddChild(aiWalkNode);
        patrolNode.AddChild(aiRotorNode);
        patrolNode.AddChild(aIPickNode);
        patrolNode.AddChild(aiAttackNode);
        return patrolNode;
    }

    protected override string CreateID()
    {
        return Config.BehaviorTreeId.Patrol;
    }
}