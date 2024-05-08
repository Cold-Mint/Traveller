using ColdMint.scripts.map.interfaces;
using Godot;

namespace ColdMint.scripts.map;

public class MapGeneratorConfig : IMapGeneratorConfig
{
    /// <summary>
    /// <para>At least how many rooms are generated</para>
    /// <para>至少生成多少个房间</para>
    /// </summary>
    public const int MinCount = 15;

    /// <summary>
    /// <para>Maximum number of rooms generated</para>
    /// <para>最多生成多少个房间</para>
    /// </summary>
    public const int MaxCount = 30;

    private int _roomCount;

    public MapGeneratorConfig(Node2D mapRoot, ulong seed)
    {
        MapRoot = mapRoot;
        Seed = seed;
        RandomNumberGenerator = new RandomNumberGenerator();
        RandomNumberGenerator.Seed = seed;
        _roomCount = RandomNumberGenerator.RandiRange(MinCount, MaxCount);
    }

    public Node2D MapRoot { get; }
    public int RoomCount => _roomCount;

    public ulong Seed { get; }
    public RandomNumberGenerator RandomNumberGenerator { get; }
}