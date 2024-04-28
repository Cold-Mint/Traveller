using Godot;

namespace ColdMint.scripts.map.interfaces;

/// <summary>
/// <para>IMapGeneratorConfig</para>
/// <para>房间生成器配置</para>
/// </summary>
public interface IMapGeneratorConfig
{
    Node2D MapRoot { get; }

    /// <summary>
    /// <para>How many rooms do we need to generate</para>
    /// <para>我们需要生成多少个房间</para>
    /// </summary>
    int RoomCount { get; }

    /// <summary>
    /// <para>seed</para>
    /// <para>种子</para>
    /// </summary>
    ulong Seed { get; }

    /// <summary>
    /// <para>RandomNumberGenerator</para>
    /// <para>随机数生成器</para>
    /// </summary>
    RandomNumberGenerator RandomNumberGenerator { get; }
}