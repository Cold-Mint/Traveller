using ColdMint.scripts.character;

namespace ColdMint.scripts.behaviorTree.ai;

/// <summary>
/// <para>The node that controls the rotor when the AI is facing the wall</para>
/// <para>当AI面向墙壁时，控制转头的节点</para>
/// </summary>
public class AIRotorNode : BehaviorTreeNodeTemplate
{
    public AICharacter Character { get; set; }

    public override int Execute(bool isPhysicsProcess, double delta)
    {
        if (Character == null)
        {
            return Config.BehaviorTreeResult.Failure;
        }

        var notFacingTheWall = Character.WallDetection.GetCollider() == null;
        if (notFacingTheWall)
        {
            return Config.BehaviorTreeResult.Failure;
        }
        else
        {
            Character.Rotor();
            return Config.BehaviorTreeResult.Success;
        }
    }
}