using System.Collections.Generic;
using System.Threading.Tasks;
using ColdMint.scripts.character;
using ColdMint.scripts.console.objectSelector;
using ColdMint.scripts.inventory;
using ColdMint.scripts.pickable;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.console.commands;

public class GiveCommand : ICommand
{
    public string Name => Config.CommandNames.Give;
    private readonly NodeTree<string> _suggest = new(null);

    public string[] GetAllSuggest(CommandArgs args)
    {
        return SuggestUtils.GetAllSuggest(args, _suggest);
    }

    public void InitSuggest()
    {
        var objectSelectorNode =
            _suggest.AddChild(
                DynamicSuggestionManager.CreateDynamicSuggestionReferenceId(Config.DynamicSuggestionID.ObjectSelector));
        objectSelectorNode.AddChild(
            DynamicSuggestionManager.CreateDynamicSuggestionReferenceId(Config.DynamicSuggestionID.Item));
    }

    public Task<bool> Execute(CommandArgs args)
    {
        if (args.Length < 2)
        {
            return Task.FromResult(false);
        }

        var inputObjectData = args.GetString(1);
        if (string.IsNullOrEmpty(inputObjectData))
        {
            return Task.FromResult(false);
        }

        //There is no need to convert to lowercase here.
        //这里不用转换为小写。
        var objectSelectorQuery = ObjectSelectorQueryRequest.Parse(inputObjectData);
        if (objectSelectorQuery == null)
        {
            return Task.FromResult(false);
        }

        var objectSelectorQueryResponse = ObjectSelector.Query(objectSelectorQuery);
        if (objectSelectorQueryResponse.Count == 0)
        {
            return Task.FromResult(false);
        }

        var inputItemId = args.GetString(2);
        if (string.IsNullOrEmpty(inputItemId))
        {
            return Task.FromResult(false);
        }

        objectSelectorQueryResponse.Filter<CharacterTemplate>(template =>
        {
            var item = ItemTypeManager.CreateItem(inputItemId, template);
            if (item == null)
            {
                return;
            }

            if (item is Node2D node2DItem)
            {
                var pickup = template.PickItem(node2DItem);
                if (!pickup)
                {
                    item.QueueFreeSelf();
                }
            }
            else
            {
                //It's not node2d that destroys itself.
                //不是node2d销毁自身。
                item.QueueFreeSelf();
            }
        });
        return Task.FromResult(false);
    }
}