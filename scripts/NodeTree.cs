using System;
using System.Collections.Generic;
using System.Linq;

namespace ColdMint.scripts;

/// <summary>
/// <para>NodeTree</para>
/// <para>节点树</para>
/// </summary>
/// <param name="data">
///<para>data</para>
///<para>数据</para>
/// </param>
/// <typeparam name="T">
///<para>data Type</para>
///<para>数据类型</para>
/// </typeparam>
public class NodeTree<T>(T? data)
{
    private readonly LinkedList<NodeTree<T>> _children = [];
    public T? Data => data;

    /// <summary>
    /// <para>AddChild</para>
    /// <para>添加子节点</para>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public NodeTree<T> AddChild(T value)
    {
        var child = new NodeTree<T>(value);
        _children.AddLast(child);
        return child;
    }

    /// <summary>
    /// <para>Gets all the child nodes (does not traverse the subtree)</para>
    /// <para>获取所有的子节点（不会遍历子树）</para>
    /// </summary>
    /// <returns></returns>
    public T[]? GetAllChildren()
    {
        List<T> childrenList = [];
        childrenList.AddRange(_children.Select(nodeTree => nodeTree.Data).OfType<T>());
        return childrenList.ToArray();
    }

    /// <summary>
    /// <para>Traversing child nodes (not traversing subtrees)</para>
    /// <para>遍历子节点（不会遍历子树）</para>
    /// </summary>
    /// <param name="callback">
    ///<para>Returns the child's callback. If false is returned within the callback method, the loop is terminated</para>
    ///<para>返回子节点的回调，若在回调方法内返回false，则终止循环</para>
    /// </param>
    public void ForEachChildren(Func<NodeTree<T>?, bool> callback)
    {
        foreach (var nodeTree in _children)
        {
            if (callback.Invoke(nodeTree) == false)
            {
                return;
            }
        }
    }

    /// <summary>
    /// <para>Remove child node</para>
    /// <para>移除子节点</para>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool RemoveChild(T value)
    {
        var node = GetChildByValue(value);
        if (node == null)
        {
            return false;
        }

        _children.Remove(node);
        return true;
    }

    /// <summary>
    /// <para>GetChild</para>
    /// <para>获取子节点</para>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public NodeTree<T>? GetChildByValue(T value)
    {
        foreach (var nodeTree in _children)
        {
            var nodeTreeData = nodeTree.Data;
            if (nodeTreeData == null)
            {
                continue;
            }

            if (nodeTreeData.Equals(value))
            {
                return nodeTree;
            }
        }

        return null;
    }

    /// <summary>
    /// <para>GetChild</para>
    /// <para>获取子节点</para>
    /// </summary>
    /// <param name="index">
    ///<para>position</para>
    ///<para>位置</para>
    /// </param>
    /// <returns></returns>
    public NodeTree<T>? GetChild(int index)
    {
        var counter = index;
        foreach (var child in _children)
        {
            if (counter == 0)
                return child;
            counter--;
        }

        return null;
    }
}