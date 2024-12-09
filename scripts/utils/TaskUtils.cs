using System.Threading.Tasks;
using Godot;

namespace ColdMint.scripts.utils;

public static class TaskUtils
{
    /// <summary>
    /// <para>Wait for the specified number of frames</para>
    /// <para>等待指定的帧数</para>
    /// </summary>
    /// <param name="node">
    ///<para>node</para>
    ///<para>节点</para>
    /// </param>
    /// <param name="framesToWait">
    ///<para>How many frames to wait for</para>
    ///<para>需要等待多少帧</para>
    /// </param>
    public static async Task WaitForFramesAsync(Node node, int framesToWait = 1)
    {
        for (var i = 0; i < framesToWait; i++)
        {
            await node.ToSignal(node.GetTree(), "process_frame");
        }
    }
}