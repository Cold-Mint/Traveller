using System.Threading.Tasks;
using ColdMint.scripts.damage;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.console.commands;

public class PlayerCommand : ICommand
{
    public string Name => Config.CommandNames.Player;
    private readonly NodeTree<string> _suggest = new(null);

    public string[] GetAllSuggest(CommandArgs args)
    {
        return SuggestUtils.GetAllSuggest(args, _suggest);
    }

    public void InitSuggest()
    {
        var indestructible = _suggest.AddChild("indestructible");
        indestructible.AddChild("get");
        var set = indestructible.AddChild("set");
        set.AddChild(
            DynamicSuggestionManager.CreateDynamicSuggestionReferenceId(Config.DynamicSuggestionID.Boolean));
        _suggest.AddChild("kill_self");
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
        var indestructibleNode = _suggest.GetChild(0);
        var killSelf = _suggest.GetChild(1)?.Data;
        if (type == indestructibleNode?.Data)
        {
            if (args.Length < 3)
            {
                return Task.FromResult(false);
            }

            var player = GameSceneDepend.Player;
            if (player == null)
            {
                return Task.FromResult(false);
            }

            var inputOp = args.GetString(2);
            if (string.IsNullOrEmpty(inputOp))
            {
                return Task.FromResult(false);
            }

            var op = inputOp.ToLowerInvariant();
            var get = indestructibleNode.GetChild(0)?.Data;
            var set = indestructibleNode.GetChild(1)?.Data;
            if (op == get)
            {
                ConsoleGui.Instance?.Print(TranslationServerUtils.TranslateWithFormat("log_get_indestructible",
                    player.Indestructible));
                return Task.FromResult(true);
            }

            if (op == set)
            {
                var value = args.GetBool(3);
                player.Indestructible = value;
                ConsoleGui.Instance?.Print(TranslationServerUtils.TranslateWithFormat("log_set_indestructible",
                    player.Indestructible));
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        if (type == killSelf)
        {
            var player = GameSceneDepend.Player;
            if (player == null)
            {
                return Task.FromResult(false);
            }

            var oldIndestructible = player.Indestructible;
            FixedDamage damage = new(player.ReadOnlyMaxHp);
            player.Indestructible = false;
            player.Damage(damage);
            player.Indestructible = oldIndestructible;
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}