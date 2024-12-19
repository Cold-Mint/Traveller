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
        switch (type)
        {
            case "g" or "get":
                ConsoleGui.Echo(MapGenerator.Seed);
                return Task.FromResult(true);
            case "rec" or "recreate":
                MapGenerator.Seed = GuidUtils.GetGuid();
                return Task.FromResult(true);
            case "s" or "set" when args.Length == 3:
            {
                var value = args[2].ToLowerInvariant();
                if (string.IsNullOrEmpty(value))
                {
                    return Task.FromResult(false);
                }

                MapGenerator.Seed = value;
                return Task.FromResult(true);
            }
            default:
                return Task.FromResult(false);
        }
    }
}