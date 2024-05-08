using ColdMint.scripts.behaviorTree.ai;
using ColdMint.scripts.character;

namespace ColdMint.scripts.behaviorTree.behavior;

/// <summary>
/// <para>Represents a behavior tree for patrol</para>
/// <para>表示巡逻的行为树</para>
/// </summary>
public class PatrolBehaviorTree : BehaviorTreeTemplate
{
    public AiCharacter? Character { get; set; }
    protected override IBehaviorTreeNode CreateRoot()
    {
        var patrolNode = new AiPatrolNode();
        var aiWalkNode = new AiWalkNode();
        var aiRotorNode = new AiRotorNode();
        var aIPickNode = new AiPickNode();
        var aiAttackNode = new AiAttackNode();
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

    protected override string? CreateId()
    {
        return Config.BehaviorTreeId.Patrol;
    }
}