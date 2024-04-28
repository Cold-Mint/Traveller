namespace ColdMint.scripts.behaviorTree;

public interface IBehaviorTree
{
    string ID { get; }

    IBehaviorTreeNode Root { get; }
}