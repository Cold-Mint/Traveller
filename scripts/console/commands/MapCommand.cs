using System.Threading.Tasks;
using ColdMint.scripts.map;

namespace ColdMint.scripts.console.commands;

/// <summary>
/// <para>MapCommand</para>
/// <para>地图相关命令</para>
/// </summary>
public class MapCommand : ICommand
{
    public string Name => Config.CommandNames.Map;

    public async Task<bool> Execute(string[] args)
    {
        if (args.Length < 2)
        {
            return false;
        }

        var type = args[1].ToLowerInvariant();
        if (type == "recreate")
        {
            await MapGenerator.GenerateMapAsync();
            return true;
        }

        return false;
    }
}