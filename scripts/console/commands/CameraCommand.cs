using System.Threading.Tasks;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.console.commands;

public class CameraCommand : ICommand
{
    public string Name => Config.CommandNames.Camera;
    private readonly NodeTree<string> _suggest = new(null);


    public string[] GetAllSuggest(CommandArgs args)
    {
        return SuggestUtils.GetAllSuggest(args, _suggest);
    }

    public void InitSuggest()
    {
        _suggest.AddChild("free");
    }

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
        var free = _suggest.GetChild(0)?.Data;
        if (type == free)
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