using System.Collections.Generic;
using ColdMint.scripts.debug;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.bubble;

/// <summary>
/// <para>BubbleMarker</para>
/// <para>气泡位置标记</para>
/// </summary>
public partial class BubbleMarker : Marker2D
{
    private readonly Dictionary<int, Node> _bubbleDictionary = [];

    /// <summary>
    /// <para>Add bubbles</para>
    /// <para>添加气泡</para>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool AddBubble(int id, Node node)
    {
        if (!_bubbleDictionary.TryAdd(id, node))
        {
            return false;
        }

        NodeUtils.HideNode(node);
        NodeUtils.CallDeferredAddChild(this, node);
        return true;
    }

    /// <summary>
    /// <para>DisplayBubble</para>
    /// <para>显示气泡</para>
    /// </summary>
    /// <remarks>
    ///<para>Display specific nodes above the creature as "bubbles", for example, question bubbles when an enemy finds the player.</para>
    ///<para>在生物头顶显示特定的节点作为“气泡”，例如：当敌人发现玩家后将显示疑问气泡。</para>
    /// </remarks>
    /// <param name="id"></param>
    public void ShowBubble(int id)
    {
        if (!_bubbleDictionary.TryGetValue(id, out var value))
        {
            LogCat.LogErrorWithFormat("bubble_not_found", LogCat.LogLabel.BubbleMarker,id);
            return;
        }

        NodeUtils.ShowNode(value);
    }

    /// <summary>
    /// <para>Hidden bubble</para>
    /// <para>隐藏气泡</para>
    /// </summary>
    public void HideBubble(int id)
    {
        if (!_bubbleDictionary.TryGetValue(id, out var value))
        {
            LogCat.LogErrorWithFormat("bubble_not_found", LogCat.LogLabel.BubbleMarker,id);
            return;
        }

        NodeUtils.HideNode(value);
    }
}