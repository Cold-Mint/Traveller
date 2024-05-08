using System.Threading.Tasks;
using Godot;

namespace ColdMint.scripts.utils;

public class NodeUtils
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
}