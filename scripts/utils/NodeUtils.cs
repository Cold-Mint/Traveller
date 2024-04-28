using Godot;

namespace ColdMint.scripts.utils;

public class NodeUtils
{
    /// <summary>
    /// <para>Delete all child nodes</para>
    /// <para>删除所有子节点</para>
    /// </summary>
    /// <param name="parent"></param>
    public static void DeleteAllChild(Node parent)
    {
        var count = parent.GetChildCount();
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                var node = parent.GetChild(0);
                node.QueueFree();
            }
        }
    }
}