﻿using System.Threading.Tasks;
using ColdMint.scripts.debug;
using ColdMint.scripts.map;
using ColdMint.scripts.map.RoomPlacer;

namespace ColdMint.scripts.console.commands;

/// <summary>
/// <para>MapCommand</para>
/// <para>地图相关命令</para>
/// </summary>
public class MapCommand : ICommand
{
    public string Name => Config.CommandNames.Map;
    public string[][] Suggest => [["recreate", "set_overlap_detection_delay"]];

    public async Task<bool> Execute(string[] args)
    {
        if (args.Length < 2)
        {
            return false;
        }

        var type = args[1].ToLowerInvariant();
        if (type == Suggest[0][0])
        {
            await MapGenerator.GenerateMapAsync();
            return true;
        }

        if (type == Suggest[0][1] && args.Length > 2)
        {
            PatchworkRoomPlacementStrategy.OverlapDetectionDelay = int.Parse(args[2]);
            ConsoleGui.Instance?.Echo(LogCat.LogWithFormat("set_overlap_detection_delay", LogCat.LogLabel.Default,
                PatchworkRoomPlacementStrategy.OverlapDetectionDelay));
            return true;
        }

        return false;
    }
}