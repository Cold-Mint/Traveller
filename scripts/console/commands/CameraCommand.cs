using System.Threading.Tasks;
using Godot;

namespace ColdMint.scripts.console.commands;

public class CameraCommand : ICommand
{
    public string Name => Config.CommandNames.Camera;
    public async Task<bool> Execute(string[] args)
    {
        if (args.Length < 2)
        {
            return false;
        }

        var type = args[1].ToLowerInvariant();
        if (type == "free")
        {
            //Camera free field of view
            //相机自由视野
            var camera2D = GameSceneDepend.Player?.Camera2D;
            if (camera2D == null)
            {
                return false;
            }
            camera2D.Position+= new Vector2(0,100);
            return true;
        }

        return false;
    }
}