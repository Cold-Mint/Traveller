using System.Threading.Tasks;
using ColdMint.scripts.map;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.console.commands;

/// <summary>
/// <para>Used to view world seeds</para>
/// <para>用于查看世界种子</para>
/// </summary>
public class SeedCommand : ICommand
{
    public string Name => Config.CommandNames.Seed;

    public Task<bool> Execute(string[] args)
    {
        if (args.Length < 2)
        {
            return Task.FromResult(false);
        }

        var type = args[1].ToLowerInvariant();
        if (type == "echo")
        {
            ConsoleGui.Echo(MapGenerator.Seed);
            return Task.FromResult(true);
        }
        else if (type == "recreate")
        {
            MapGenerator.Seed = GuidUtils.GetGuid();
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}