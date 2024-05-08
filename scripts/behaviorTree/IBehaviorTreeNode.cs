
namespace ColdMint.scripts.behaviorTree;

/// <summary>
/// <para>Behavior tree node</para>
/// <para>行为树节点</para>
/// </summary>
public interface IBehaviorTreeNode
{
    /// <summary>
    /// <para>execution node</para>
    /// <para>执行节点</para>
    /// </summary>
    /// <paramref name="isPhysicsProcess">
    ///<para>Whether to call within a physical process</para>
    ///<para>是否在物理流程内调用</para>
    /// </paramref>
    int Execute(bool isPhysicsProcess, double delta);

    /// <summary>
    /// <para>The parent of this node</para>
    /// <para>此节点的父节点</para>
    /// </summary>
    IBehaviorTreeNode? Parent { get; set; }

    /// <summary>
    /// <para>child node</para>
    /// <para>子节点</para>
    /// </summary>
    IBehaviorTreeNode[] Children { get; }
}