using System.Threading.Tasks;
using ColdMint.scripts.debug;
using ColdMint.scripts.map;
using ColdMint.scripts.map.RoomPlacer;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.console.commands;

/// <summary>
/// <para>MapCommand</para>
/// <para>地图相关命令</para>
/// </summary>
public class MapCommand : ICommand
{
    public string Name => Config.CommandNames.Map;
    private readonly NodeTree<string> _suggest = new(null);


    public string[] GetAllSuggest(CommandArgs args)
    {
        return SuggestUtils.GetAllSuggest(args, _suggest);
    }

    public void InitSuggest()
    {
        _suggest.AddChild("recreate");
        var setOverlapDetectionDelay = _suggest.AddChild("set_overlap_detection_delay");
        setOverlapDetectionDelay.AddChild("reset");
        var level = _suggest.AddChild("level");
        level.AddChild("get");
        level.AddChild("set");
    }

    public async Task<bool> Execute(CommandArgs args)
    {
        if (args.Length < 2)
        {
            return false;
        }

        var inputType = args.GetString(1);
        if (string.IsNullOrEmpty(inputType))
        {
            return false;
        }

        var type = inputType.ToLowerInvariant();
        var recreate = _suggest.GetChild(0)?.Data;
        if (type == recreate)
        {
            await MapGenerator.GenerateMapAsync();
            return true;
        }

        var setOverlapDetectionDelay = _suggest.GetChild(1)?.Data;
        if (type == setOverlapDetectionDelay)
        {
            PatchworkRoomPlacementStrategy.OverlapDetectionDelay = args.GetInt(2, Config.DefaultOverlapDetectionDelay);
            ConsoleGui.Instance?.Print(TranslationServerUtils.TranslateWithFormat("log_set_overlap_detection_delay",
                PatchworkRoomPlacementStrategy.OverlapDetectionDelay));
            return true;
        }

        var levelTree = _suggest.GetChild(2);
        if (type == levelTree?.Data)
        {
            var op = args.GetString(2);
            var get = levelTree.GetChild(0)?.Data;
            var set = levelTree.GetChild(1)?.Data;
            if (op == get)
            {
                ConsoleGui.Instance?.Print(TranslationServerUtils.TranslateWithFormat("log_get_level",
                    MapGenerator.Level));
                return true;
            }

            if (op == set)
            {
                var value = args.GetInt(3);
                MapGenerator.Level = value;
                ConsoleGui.Instance?.Print(TranslationServerUtils.TranslateWithFormat("log_set_level",
                    MapGenerator.Level));
                return true;
            }
        }

        return false;
    }
}