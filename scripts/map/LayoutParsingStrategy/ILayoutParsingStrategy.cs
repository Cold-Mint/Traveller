using System.Threading.Tasks;
using ColdMint.scripts.levelGraphEditor;

namespace ColdMint.scripts.map.LayoutParsingStrategy;

/// <summary>
/// <para>Layout parsing strategy</para>
/// <para>布局图解析策略</para>
/// </summary>
public interface ILayoutParsingStrategy
{
    /// <summary>
    /// <para>Sets the layout diagram to parse</para>
    /// <para>设置要解析的布局图</para>
    /// </summary>
    /// <param name="levelGraphEditorSaveData"></param>
    public void SetLevelGraph(LevelGraphEditorSaveData levelGraphEditorSaveData);
    
    /// <summary>
    /// <para>Gets the next room to place</para>
    /// <para>获取下一个要放置的房间</para>
    /// </summary>
    /// <returns></returns>
    public Task<RoomNodeData?> Next();

    /// <summary>
    /// <para>Gets the ID of the next parent node to place</para>
    /// <para>获取下一个要放置的父节点ID</para>
    /// </summary>
    /// <returns></returns>
    public Task<string?> GetNextParentNodeId();
    
    /// <summary>
    /// <para>Is there another room that needs to be placed</para>
    /// <para>是否还有下一个需要放置的房间</para>
    /// </summary>
    /// <returns></returns>
    public Task<bool> HasNext();
}