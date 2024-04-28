using ColdMint.scripts.character;

namespace ColdMint.scripts.behaviorTree.ai;

/// <summary>
/// <para>一个节点用于实现角色的移动</para>
/// <para>A node is used to implement the movement of the character</para>
/// </summary>
public class AIWalkNode : BehaviorTreeNodeTemplate
{
    public AICharacter Character { get; set; }

    public override int Execute(bool isPhysicsProcess, double delta)
    {
        if (Character == null)
        {
            return Config.BehaviorTreeResult.Failure;
        }

        if (Character.FacingLeft)
        {
            //If the character is facing left, move left
            //如果角色面向左边，那么向左移动
            Character.MoveLeft();
        }
        else
        {
            Character.MoveRight();
        }

        return Config.BehaviorTreeResult.Success;
    }
}