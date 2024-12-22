using System.Threading.Tasks;

namespace ColdMint.scripts.console.commands;

public class CameraCommand : ICommand
{
    public string Name => Config.CommandNames.Camera;

    public string[][] Suggest =>
    [
        ["free"]
    ];

    public async Task<bool> Execute(string[] args)
    {
        if (args.Length < 2)
        {
            return false;
        }

        var type = args[1].ToLowerInvariant();
        if (type is "f" or "free")
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