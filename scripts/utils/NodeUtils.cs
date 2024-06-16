using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColdMint.scripts.debug;
using ColdMint.scripts.inventory;
using ColdMint.scripts.item;
using ColdMint.scripts.item.weapon;
using Godot;
using PacksackUi = ColdMint.scripts.loader.uiLoader.PacksackUi;


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

    /// <summary>
    /// <para>Find the corresponding container node based on the child node</para>
    /// <para>根据子节点查找对应的容器节点</para>
    /// </summary>
    /// <remarks>
    ///<para>We want child nodes to be placed under a specific parent node to facilitate the same management. For example, the weapon node should be placed inside the Weapon container node. We call a parent node of the same type as a child node a "container". This method is used to find the corresponding node container based on the type of the child node.</para>
    ///<para>我们希望子节点被放置在特定的父节点下，方便同一管理。例如：武器节点应该被放置在“武器容器”节点内。我们将子节点的类型相同的父节点叫做“容器”。此方法用于根据子节点的类型查找对应的节点容器。</para>
    /// </remarks>
    /// <param name="childNode">
    ///<para>childNode</para>
    ///<para>子节点</para>
    /// </param>
    /// <param name="defaultParentNode">
    ///<para>Default parent, which returns the default node if it cannot be matched by type.</para>
    ///<para>默认父节点，当按照类型无法匹配时，将返回默认节点。</para>
    /// </param>
    /// <returns></returns>
    public static Node FindContainerNode(Node childNode, Node defaultParentNode)
    {
        if (GameSceneNodeHolder.WeaponContainer != null && childNode is WeaponTemplate)
        {
            return GameSceneNodeHolder.WeaponContainer;
        }

        if (GameSceneNodeHolder.PacksackContainer!= null && childNode is Packsack)
        {
            return GameSceneNodeHolder.PacksackContainer;
        }

        if (GameSceneNodeHolder.BackpackUiContainer!=null && childNode is PacksackUi)
        {
            return GameSceneNodeHolder.BackpackUiContainer;
        }
        return defaultParentNode;
    }

    /// <summary>
    /// <para>Instantiate Packed Scene</para>
    /// <para>实例化场景</para>
    /// </summary>
    /// <remarks>
    ///<para>This method is recommended in place of all packedScene.Instantiate() calls within a project, using it to instantiate a scene, optionally assigned to a container that matches the type of the root node.</para>
    ///<para>推荐使用此方法代替项目内所有的packedScene.Instantiate()调用，使用此方法实例化场景，可选择将其分配到与根节点类型相匹配的容器内。</para>
    /// </remarks>
    /// <param name="packedScene">
    ///<para>packedScene</para>
    ///<para>打包的场景</para>
    /// </param>
    /// <param name="assignedByRootNodeType">
    ///<para>Enabled by default, whether to place a node into a container node that matches the type of the root node after it is instantiated. If the assignment fails by type, it is placed under the default parent node.</para>
    ///<para>默认启用，实例化节点后，是否将其放置到与根节点类型相匹配的容器节点内。如果按照类型分配失败，则放置在默认父节点下。</para>
    /// </param>
    /// <param name="defaultParentNode">
    /// <para>Default parent. If null is passed, the instantiated node is not put into the parent. You need to place it manually, which is quite useful for delaying setting the parent node.</para>
    /// <para>默认父节点，如果传入null，则不会将实例化后的节点放入父节点内。您需要手动放置，这对于延迟设置父节点相当有用。</para>
    /// </param>
    /// <returns></returns>
    public static T? InstantiatePackedScene<T>(PackedScene packedScene, Node? defaultParentNode = null,
        bool assignedByRootNodeType = true) where T : Node
    {
        try
        {
            var instantiateNode = packedScene.Instantiate<T>();
            //An attempt is made to place an instantiated node under a husband node only after the default parent node is set.
            //只有设定了默认父节点后才会尝试将实例化的节点放置到夫节点下。
            if (defaultParentNode != null)
            {
                if (assignedByRootNodeType)
                {
                    var containerNode = FindContainerNode(instantiateNode, defaultParentNode);
                    containerNode.AddChild(instantiateNode);
                }
                else
                {
                    //If you do not need to assign by type, place it under the default parent node.
                    //如果不需要按照类型分配，那么将其放到默认父节点下。
                    defaultParentNode.AddChild(instantiateNode);
                }
            }

            return instantiateNode;
        }
        catch (InvalidCastException e)
        {
            //null is returned if the type conversion fails.
            //在类型转换失败时，返回null。
            LogCat.WhenCaughtException(e);
            return null;
        }
    }
}