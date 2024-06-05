using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace ColdMint.scripts.utils;

/// <summary>
/// <para>Node Utils</para>
/// <para>节点工具</para>
/// </summary>
public static class NodeUtils
{
    /// <summary>
    /// <para>Delete all child nodes</para>
    /// <para>删除所有子节点</para>
    /// </summary>
    /// <param name="parent"></param>
    public static int DeleteAllChild(Node parent)
    {
        var deleteNumber = 0;
        var count = parent.GetChildCount();
        if (count <= 0) return deleteNumber;
        for (var i = 0; i < count; i++)
        {
            var node = parent.GetChild(0);
            parent.RemoveChild(node);
            node.QueueFree();
            deleteNumber++;
        }

        return deleteNumber;
    }

    /// <summary>
    /// <para>All child nodes are removed asynchronously</para>
    /// <para>异步删除所有子节点</para>
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static async Task<int> DeleteAllChildAsync(Node parent)
    {
        return await Task.Run(() => DeleteAllChild(parent));
    }

    /// <summary>
    /// <para>Gets the node closest to the origin</para>
    /// <para>获取距离原点最近的节点</para>
    /// </summary>
    /// <param name="origin">
    ///<para>origin</para>
    ///<para>原点</para>
    /// </param>
    /// <param name="array">
    ///<para>Node array</para>
    ///<para>节点数组</para>
    /// </param>
    /// <param name="exclude">
    ///<para>Which nodes are excluded</para>
    ///<para>排除哪些节点</para>
    /// </param>
    /// <param name="excludeInvisibleNodes">
    ///<para>Whether or not unseen nodes should be excluded</para>
    ///<para>是否排除不可见的节点</para>
    /// </param>
    /// <returns></returns>
    public static Node2D? GetTheNearestNode(Node2D origin, Node[] array, HashSet<Node>? exclude = null,
        bool excludeInvisibleNodes = true)
    {
        var closestDistance = float.MaxValue;
        Node2D? closestNode = null;
        foreach (var node in array)
        {
            if (node is not Node2D node2D) continue;
            if (excludeInvisibleNodes && !node2D.Visible)
            {
                //If invisible nodes are excluded and the current node is invisible, then the next.
                //如果排除不可见的节点，且当前节点就是不可见的，那么下一个。
                continue;
            }

            if (exclude != null && exclude.Contains(node))
            {
                //If the current node, is within our exclusion project. So the next one.
                //如果当前节点，在我们的排除项目内。那么下一个。
                continue;
            }

            var distance = node2D.GlobalPosition.DistanceTo(origin.GlobalPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node2D;
            }
        }

        return closestNode;
    }
}