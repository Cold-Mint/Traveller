namespace ColdMint.scripts.behaviorTree.framework;

/// <summary>
/// <para>SequenceNode</para>
/// <para>序列器节点</para>
/// </summary>
/// <remarks>
///<para>Execute all its child nodes in turn, that is, after the current one returns to the "finished" state, run the second child node, until all nodes return "finished", then this node returns "finished".</para>
///<para>将其所有子节点依次执行，也就是说当前一个返回“完成”状态后，再运行第二个子节点，直到所有节点都返回“完成”后，那么此节点返回"完成"</para>
/// </remarks>
public class SequenceNode : BehaviorTreeNodeTemplate
{
    /// <summary>
    /// <para>Check whether all child nodes are executed in sequence</para>
    /// <para>所有子节点是否按顺序执行完毕</para>
    /// </summary>
    bool _complete = true;

    public override int Execute(bool isPhysicsProcess, double delta)
    {
        if (Children.Length == 0)
        {
            return Config.BehaviorTreeResult.Failure;
        }

        if (_complete)
        {
            //If the last execution is over, we start executing a new sequence
            //如果上次执行完毕了，我们开始执行新的序列
            _complete = false;
        }
        else
        {
            //If it hasn't finished, we return to Running
            //如果还没有执行完毕，我们返回Running
            return Config.BehaviorTreeResult.Running;
        }

        var result = true;
        foreach (var behaviorTreeNode in Children)
        {
            var singleResult = behaviorTreeNode.Execute(isPhysicsProcess, delta);
            while (singleResult == Config.BehaviorTreeResult.Running)
            {
                //Wait for the child node to complete execution
                //等得子节点执行完毕
            }

            //Single child node is executed
            //单个子节点执行完毕
            if (singleResult == Config.BehaviorTreeResult.Failure)
            {
                //If a child node fails, the entire sequence fails
                //如果有一个子节点失败，整个序列失败
                result = false;
            }
        }

        //All child nodes are executed
        //全部子节点执行完毕
        _complete = true;
        if (result)
        {
            return Config.BehaviorTreeResult.Success;
        }
        else
        {
            return Config.BehaviorTreeResult.Failure;
        }
    }
}