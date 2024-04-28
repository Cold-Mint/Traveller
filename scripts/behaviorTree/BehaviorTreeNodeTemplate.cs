using System.Collections.Generic;

namespace ColdMint.scripts.behaviorTree;

/// <summary>
/// <para>Behavior tree node template</para>
/// <para>行为树节点模板</para>
/// </summary>
public abstract class BehaviorTreeNodeTemplate : IBehaviorTreeNode
{
    private List<IBehaviorTreeNode> _children = new List<IBehaviorTreeNode>();

    public void AddChild(IBehaviorTreeNode child)
    {
        _children.Add(child);
        child.Parent = this;
    }

    public void RemoveChild(IBehaviorTreeNode child)
    {
        _children.Remove(child);
        child.Parent = null;
    }

    /// <summary>
    /// <para>Gets the child node of the specified type</para>
    /// <para>获取指定类型的子节点</para>
    /// </summary>
    /// <param name="defaultT"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetChild<T>(T defaultT)
    {
        if (_children.Count == 0)
        {
            return defaultT;
        }

        foreach (var behaviorTreeNode in _children)
        {
            if (behaviorTreeNode is T t)
            {
                return t;
            }
        }

        return defaultT;
    }

    public abstract int Execute(bool isPhysicsProcess, double delta);

    public IBehaviorTreeNode Parent { get; set; }
    public IBehaviorTreeNode[] Children => _children.ToArray();
}