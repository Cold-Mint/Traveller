using System.Collections.Generic;
using Godot;

namespace ColdMint.scripts.console.objectSelector;

public class PlayerDataSource : IObjectSelectorDataSource
{
    public int ObjectType => Config.ObjectType.Player;

    public void Query(ObjectSelectorQueryRequest request, ref List<Node> resultList)
    {
        var player = GameSceneDepend.Player;
        if (player != null)
        {
            resultList.Add(player);
        }
    }
}