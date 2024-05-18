using System.Threading.Tasks;
using ColdMint.scripts.levelGraphEditor;

namespace ColdMint.scripts.map;

/// <summary>
/// <para>Map generator</para>
/// <para>地图生成器</para>
/// </summary>
/// <remarks>
///<para>Responsible for the overall map generation process control</para>
///<para>负责地图的整体生成流程控制</para>
/// </remarks>
public static class MapGenerator
{
    /// <summary>
    /// <para>Layout map selection strategy</para>
    /// <para>布局图选择策略</para>
    /// </summary>
    private static ILayoutStrategy? _layoutStrategy;

    public static ILayoutStrategy? LayoutStrategy
    {
        get => _layoutStrategy;
        set => _layoutStrategy = value;
    }

    /// <summary>
    /// <para>Generating a map</para>
    /// <para>生成地图</para>
    /// </summary>
    public static async Task GenerateMap()
    {
        if (_layoutStrategy == null)
        {
            return;
        }

        //Get the layout data
        //拿到布局图数据
        var levelGraphEditorSaveData = await _layoutStrategy.GetLayout();
        //Finding the starting room
        //查找起点房间
        if (levelGraphEditorSaveData.RoomNodeDataList == null || levelGraphEditorSaveData.RoomNodeDataList.Count == 0)
        {
            return;
        }

        var startRoomNodeData = levelGraphEditorSaveData.RoomNodeDataList.Find(roomNodeData =>
            roomNodeData.HasTag(Config.RoomDataTag.StartingRoom));
        if (startRoomNodeData == null)
        {
            //Can't find the starting room
            //找不到起点房间
            return;
        }
        //The starting room is regarded as the root node, and the map is generated from the root node to the leaf node like the tree structure.
        //TODO:将起点房间看作根节点，像树结构一样，从根节点到叶节点生成地图。
    }
}