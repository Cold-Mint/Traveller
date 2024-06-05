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
    private const string Path = "res://data/levelGraphs/test.json";

    public Task<LevelGraphEditorSaveData?> GetLayout()
    {
        var exists = FileAccess.FileExists(Path);
        if (!exists)
        {
            return Task.FromResult<LevelGraphEditorSaveData?>(null);
        }

        var json = FileAccess.GetFileAsString(Path);
        if (json == null)
        {
            return Task.FromResult<LevelGraphEditorSaveData?>(null);
        }

        return Task.FromResult(JsonSerialization.Deserialize<LevelGraphEditorSaveData>(json));
    }
}