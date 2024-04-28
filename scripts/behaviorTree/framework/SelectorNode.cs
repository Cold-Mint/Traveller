namespace ColdMint.scripts.behaviorTree.framework;

/// <summary>
/// <para>Selector node</para>
/// <para>选择器节点</para>
/// </summary>
/// <remarks>
///<para>Select an execution of the child node and pass the execution result to the parent node</para>
///<para>选择其子节点的某一个执行，并将执行结果传递给父节点</para>
/// </remarks>
public abstract class SelectorNode : BehaviorTreeNodeTemplate
{
    public override int Execute(bool isPhysicsProcess, double delta)
    {
        var behaviorTreeNode = SelectNode(isPhysicsProcess, delta, Children);
        if (behaviorTreeNode == null)
        {
            return Config.BehaviorTreeResult.Failure;
        }

        return behaviorTreeNode.Execute(isPhysicsProcess, delta);
    }

    
    
    /// <summary>
    /// <para>Select an abstract method for the node</para>
    /// <para>选择节点的抽象方法</para>
    /// </summary>
    /// <returns></returns>
    public abstract IBehaviorTreeNode SelectNode(bool isPhysicsProcess, double delta, IBehaviorTreeNode[] children);
}