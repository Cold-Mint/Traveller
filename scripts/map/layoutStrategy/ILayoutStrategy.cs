using System.Threading.Tasks;
using ColdMint.scripts.levelGraphEditor;

namespace ColdMint.scripts.map.layoutStrategy;

public interface ILayoutStrategy
{
    /// <summary>
    /// <para>Get layout</para>
    /// <para>获取布局图</para>
    /// </summary>
    /// <returns></returns>
    public Task<LevelGraphEditorSaveData?> GetLayout();
}