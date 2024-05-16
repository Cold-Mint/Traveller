using System.Collections.Generic;

namespace ColdMint.scripts.levelGraphEditor;

/// <summary>
/// <para>Level map editor saves data</para>
/// <para>关卡图编辑器保存的数据</para>
/// </summary>
public class LevelGraphEditorSaveData
{
    public List<ConnectionData>? ConnectionDataList { get; set; }

    public List<RoomNodeData>? RoomNodeDataList { get; set; }
}