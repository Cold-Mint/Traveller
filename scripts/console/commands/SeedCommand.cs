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
    public string[][] Suggest => [["get", "recreate", "set"]];

    public Task<bool> Execute(CommandArgs args)
    {
        if (args.Length < 2)
        {
            return Task.FromResult(false);
        }

        var inputType = args.GetString(1);
        if (string.IsNullOrEmpty(inputType))
        {
            return Task.FromResult(false);
        }

        var type = inputType.ToLowerInvariant();
        if (type == Suggest[0][0])
        {
            ConsoleGui.Instance?.Echo(MapGenerator.Seed);
            return Task.FromResult(true);
        }

        if (type == Suggest[0][1])
        {
            MapGenerator.Seed = GuidUtils.GetGuid();
            return Task.FromResult(true);
        }

        if (type == Suggest[0][2])
        {
            var value = args.GetString(2);
            if (string.IsNullOrEmpty(value))
            {
                return Task.FromResult(false);
            }

            MapGenerator.Seed = value;
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}