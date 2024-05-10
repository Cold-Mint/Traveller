using ColdMint.scripts.debug;
using ColdMint.scripts.map.interfaces;
using Godot;

namespace ColdMint.scripts.map;

public class MapGeneratorConfig : IMapGeneratorConfig
{
    /// <summary>
    /// <para>At least how many rooms are generated</para>
    /// <para>至少生成多少个房间</para>
    /// </summary>
    public const int MinRoomCount = 15;

    /// <summary>
    /// <para>Maximum number of rooms generated</para>
    /// <para>最多生成多少个房间</para>
    /// </summary>
    public const int MaxRoomCount = 30;


    public const int MinBranchCount = 3;

    public const int MaxBranchCount = 5;

    private int _roomCount;
    private int _branchCount;

    public MapGeneratorConfig(Node2D mapRoot, ulong seed)
    {
        MapRoot = mapRoot;
        Seed = seed;
        RandomNumberGenerator = new RandomNumberGenerator();
        RandomNumberGenerator.Seed = seed;
        _roomCount = RandomNumberGenerator.RandiRange(MinRoomCount, MaxRoomCount);
        _branchCount = RandomNumberGenerator.RandiRange(MinBranchCount, MaxBranchCount);
        LogCat.Log("Seed:" + seed + " RoomCount:" + _roomCount);
    }

    public Node2D MapRoot { get; }
    public int RoomCount => _roomCount;
    public int BranchCount => _branchCount;

    public ulong Seed { get; }
    public RandomNumberGenerator RandomNumberGenerator { get; }
}