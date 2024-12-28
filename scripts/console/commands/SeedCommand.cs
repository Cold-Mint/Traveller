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
    private readonly NodeTree<string> _suggest = new(null);

    public string[] GetAllSuggest(CommandArgs args)
    {
        return SuggestUtils.GetAllSuggest(args, _suggest);
    }

    public void InitSuggest()
    {
        _suggest.AddChild("get");
        _suggest.AddChild("set");
        _suggest.AddChild("recreate");
    }

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
        var get = _suggest.GetChild(0)?.Data;
        if (type == get)
        {
            ConsoleGui.Instance?.Echo(MapGenerator.Seed);
            return Task.FromResult(true);
        }

        var set = _suggest.GetChild(1)?.Data;
        if (type == set)
        {
            var value = args.GetString(2);
            if (string.IsNullOrEmpty(value))
            {
                return Task.FromResult(false);
            }

            MapGenerator.Seed = value;
            return Task.FromResult(true);
        }

        var recreate = _suggest.GetChild(2)?.Data;
        if (type == recreate)
        {
            MapGenerator.Seed = GuidUtils.GetGuid();
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}