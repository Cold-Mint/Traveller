namespace ColdMint.scripts.behaviorTree;

public interface IBehaviorTree
{
    string? Id { get; }

    IBehaviorTreeNode? Root { get; }
}