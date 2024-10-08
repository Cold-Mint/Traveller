using Godot;

namespace ColdMint.scripts.map.room;

/// <summary>
/// <para>The tag of the generated entity</para>
/// <para>生成实体的标记</para>
/// </summary>
public interface ISpawnMarker
{
    /// <summary>
    /// <para>Generating entity</para>
    /// <para>生成实体</para>
    /// </summary>
    /// <param name="waveNumber">
    ///<para>Spawning waves</para>
    ///<para>刷怪的波次</para>
    /// </param>
    /// <remarks>
    ///<para>Return the result of the generation. If null is returned, the generation fails.</para>
    ///<para>返回生成结果，为null则生成失败。</para>
    /// </remarks>
    Node2D? Spawn(int waveNumber);

    /// <summary>
    /// <para>GetMaxWaveNumber</para>
    /// <para>获取生成器支持的最大生成波数</para>
    /// </summary>
    /// <returns></returns>
    int GetMaxWaveNumber();

    /// <summary>
    /// <para>Can Queue Free</para>
    /// <para>可释放节点吗</para>
    /// </summary>
    /// <returns></returns>
    bool CanQueueFree();

    /// <summary>
    /// <para>Execute Queue Free</para>
    /// <para>执行释放节点</para>
    /// </summary>
    void DoQueueFree();
}