using System.Threading.Tasks;

namespace ColdMint.scripts.console.commands;

public class FogCommand : ICommand
{
    public string Name => Config.CommandNames.Fog;
    public string[][] Suggest => [["visible"]];
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
        if (type == Suggest[0][0])
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