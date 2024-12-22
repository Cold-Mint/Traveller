using System.Threading.Tasks;

namespace ColdMint.scripts.console.commands;

public class FogCommand : ICommand
{
    public string Name => Config.CommandNames.Fog;
    public string[][] Suggest => [["visible"]];

    public async Task<bool> Execute(string[] args)
    {
        if (args.Length < 2)
        {
            return false;
        }

        var type = args[1].ToLowerInvariant();
        if (type is "v" or "visible")
        {
            var fog = GameSceneDepend.Fog;
            if (fog == null)
            {
                return false;
            }

            fog.Visible = !fog.Visible;
            return true;
        }

        return false;
    }
}