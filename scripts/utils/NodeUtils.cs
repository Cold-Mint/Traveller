using System;
using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using ColdMint.scripts.projectile;
using Godot;
using Packsack = ColdMint.scripts.inventory.Packsack;
using PacksackUi = ColdMint.scripts.loader.uiLoader.PacksackUi;
using WeaponTemplate = ColdMint.scripts.weapon.WeaponTemplate;


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
    /// <para>Call the method to set the parent node at leisure</para>
    /// <para>在空闲时刻调用设置父节点的方法</para>
    /// </summary>
    /// <param name="parentNode"></param>
    /// <param name="childNode"></param>
    public static void CallDeferredReparent(Node parentNode, Node childNode)
    {
        childNode.CallDeferred(Node.MethodName.Reparent, parentNode);
    }


    /// <summary>
    /// <para>ShowNode</para>
    /// <para>显示节点</para>
    /// </summary>
    /// <param name="node">
    ///<para>node</para>
    ///<para>节点</para>
    /// </param>
    /// <returns>
    ///<para>Is it displayed successfully?</para>
    ///<para>是否显示成功</para>
    /// </returns>
    public static bool ShowNode(Node node)
    {
        if (node is Node2D node2D)
        {
            node2D.Show();
            return true;
        }

        if (node is CanvasItem canvasItem)
        {
            canvasItem.Show();
            return true;
        }

        return false;
    }

    /// <summary>
    /// <para>hidden node</para>
    /// <para>隐藏节点</para>
    /// </summary>
    /// <param name="node">
    ///<para>Node to hide</para>
    ///<para>要隐藏的节点</para>
    /// </param>
    /// <returns>
    ///<para>Hide success or not</para>
    ///<para>是否隐藏成功</para>
    /// </returns>
    public static bool HideNode(Node node)
    {
        if (node is Node2D node2D)
        {
            node2D.Hide();
            return true;
        }

        if (node is CanvasItem canvasItem)
        {
            canvasItem.Hide();
            return true;
        }

        return false;
    }


    /// <summary>
    /// <para>Sets child nodes for a node</para>
    /// <para>为某个节点设置子节点</para>
    /// </summary>
    /// <param name="parentNode"></param>
    /// <param name="childNode"></param>
    public static void CallDeferredAddChild(Node parentNode, Node childNode)
    {
        parentNode.CallDeferred("add_child", childNode);
    }

    /// <summary>
    /// <para>Traverse the child nodes of type T under the parent node</para>
    /// <para>遍历父节点下T类型的子节点</para>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="func">
    ///<para>A function that handles callbacks and returns true to terminate the traversal of the node</para>
    ///<para>用于处理回调的函数，返回true终止遍历节点</para>
    /// </param>
    /// <typeparam name="T">
    ///<para>When the type is specified as Node, all child nodes are returned.</para>
    ///<para>当指定类型为Node时，将返回所有子节点。</para>
    /// </typeparam>
    public static void ForEachNode<T>(Node parent, Func<T, bool> func) where T : Node
    {
        var count = parent.GetChildCount();
        if (count <= 0)
        {
            return;
        }

        for (var i = 0; i < count; i++)
        {
            var node = parent.GetChild(i);
            if (node is not T t) continue;
            if (func.Invoke(t))
            {
                break;
            }
        }
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
    /// <param name="excludeInvisibleNodes">
    ///<para>Whether or not unseen nodes should be excluded</para>
    ///<para>是否排除不可见的节点</para>
    /// </param>
    /// <param name="filter">
    ///<para>Filter, which returns true within the function to filter the specified node.</para>
    ///<para>过滤器，在函数内返回true，则过滤指定节点。</para>
    /// </param>
    /// <returns></returns>
    public static Node2D? GetTheNearestNode(Node2D origin, Node[] array,
        bool excludeInvisibleNodes = true, Func<Node2D, bool>? filter = null)
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

            if (filter != null && filter.Invoke(node2D))
            {
                //If there is a filter, and the filter returns true, then the next.
                //如果有过滤器，且过滤器返回true，那么下一个。
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
        if (GameSceneDepend.AiCharacterContainer!= null && childNode is AiCharacter)
        {
            return GameSceneDepend.AiCharacterContainer;
        }
        
        if (GameSceneDepend.ProjectileContainer != null && childNode is Projectile)
        {
            return GameSceneDepend.ProjectileContainer;
        }

        if (GameSceneDepend.WeaponContainer != null && childNode is WeaponTemplate)
        {
            return GameSceneDepend.WeaponContainer;
        }

        if (GameSceneDepend.PacksackContainer != null && childNode is Packsack)
        {
            return GameSceneDepend.PacksackContainer;
        }

        if (GameSceneDepend.BackpackUiContainer != null && childNode is PacksackUi)
        {
            return GameSceneDepend.BackpackUiContainer;
        }

        return defaultParentNode;
    }

    /// <summary>
    /// <para>Instantiate the scene and transform it into a node of the target type</para>
    /// <para>实例化场景并将其转换为目标类型的节点</para>
    /// </summary>
    /// <param name="packedScene">
    ///<para>packedScene</para>
    ///<para>打包的场景</para>
    /// </param>
    /// <typeparam name="T">
    ///<para>genericity</para>
    ///<para>泛型</para>
    /// </typeparam>
    /// <returns>
    ///<para>If the returned type is the target type, the node converted to the target type is returned, otherwise null is returned</para>
    ///<para>如果返回的类型是目标类型，那么返回转换到目标类型的节点，否则返回null</para>
    /// </returns>
    public static T? InstantiatePackedScene<T>(PackedScene packedScene)
        where T : class
    {
        var node = packedScene.Instantiate();
        // Check the type conversion and return the result successfully
        // 检查类型转化，成功返回结果
        if (node is T result) return result;
        // If the transformation fails, release the created node
        //如果转型失败，释放所创建的节点
        LogCat.LogWarningWithFormat("warning_node_cannot_cast_to", LogCat.LogLabel.Default, LogCat.UploadFormat, node,
            nameof(T));
        node.QueueFree();
        return null;
    }
}