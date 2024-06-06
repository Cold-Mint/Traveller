namespace ColdMint.scripts.behaviorTree;

/// <summary>
/// <para>IBehavior Tree</para>
/// <para>行为树</para>
/// </summary>
public interface IBehaviorTree
{
    string? Id { get; }

    IBehaviorTreeNode? Root { get; }
}