using System.Threading.Tasks;

namespace ColdMint.scripts.console.commands;

public class CameraCommand : ICommand
{
    public string Name => Config.CommandNames.Camera;

    public string[][] Suggest =>
    [
        ["free"]
    ];

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
            //Camera free field of view
            //相机自由视野
            var camera2D = GameSceneDepend.Player?.Camera2D;
            if (camera2D == null)
            {
                return false;
            }

            camera2D.FreeVision = !camera2D.FreeVision;
            return true;
        }

        return false;
    }
}