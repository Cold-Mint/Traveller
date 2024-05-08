using Godot;

namespace ColdMint.scripts.behaviorTree;

/// <summary>
/// <para>BehaviorNode</para>
/// <para>行为节点</para>
/// </summary>
public partial class BehaviorNode : Node2D
{
    public IBehaviorTreeNode? Root { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        InvokeBehaviorTreeNode(true, delta);
    }

    // public override void _Process(double delta)
    // {
    //     InvokeBehaviorTreeNode(false, delta);
    // }

    /// <summary>
    /// <para>InvokeBehaviorTreeNode</para>
    /// <para>调用行为树节点</para>
    /// </summary>
    private void InvokeBehaviorTreeNode(bool isPhysicsProcess, double delta)
    {
        if (Root == null)
        {
            return;
        }
        Root.Execute(isPhysicsProcess, delta);
    }
}