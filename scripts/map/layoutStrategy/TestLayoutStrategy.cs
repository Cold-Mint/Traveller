using System.Threading.Tasks;
using ColdMint.scripts.levelGraphEditor;
using ColdMint.scripts.serialization;
using Godot;

namespace ColdMint.scripts.map.layoutStrategy;

/// <summary>
/// <para>Layout selection strategy to use at test time</para>
/// <para>测试时使用的布局选择策略</para>
/// </summary>
public class TestLayoutStrategy : ILayoutStrategy
{
    private string _path = "res://data/levelGraphs/test.json";

    public Task<LevelGraphEditorSaveData?> GetLayout()
    {
        var exists = FileAccess.FileExists(_path);
        if (!exists)
        {
            return Task.FromResult<LevelGraphEditorSaveData?>(null);
        }

        var json = FileAccess.GetFileAsString(_path);
        if (json == null)
        {
            return Task.FromResult<LevelGraphEditorSaveData?>(null);
        }

        return Task.FromResult(JsonSerialization.Deserialize<LevelGraphEditorSaveData>(json));
    }
}