namespace ColdMint.scripts.behaviorTree.framework;

/// <summary>
/// <para>并行节点</para>
/// <para>ParallelNode</para>
/// </summary>
/// <remarks>
///<para>Run all of its child nodes</para>
///<para>将其所有子节点都运行一遍</para>
/// </remarks>
public class ParallelNode : BehaviorTreeNodeTemplate
{
    public override int Execute(bool isPhysicsProcess, double delta)
    {
        foreach (var child in Children)
        {
            child.Execute(isPhysicsProcess, delta);
        }

        return Config.BehaviorTreeResult.Success;
    }
}