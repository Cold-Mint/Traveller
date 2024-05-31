using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using ColdMint.scripts.loader.sceneLoader;
using ColdMint.scripts.map.events;
using Godot;

namespace ColdMint.scripts.map;

/// <summary>
/// <para>PlayerSpawn</para>
/// <para>玩家出生点</para>
/// </summary>
public partial class PlayerSpawn : Marker2D
{
    private PackedScene? _playerPackedScene;

    public override void _Ready()
    {
        base._Ready();
        _playerPackedScene = GD.Load<PackedScene>("res://prefab/entitys/Character.tscn");
        MapGenerator.MapGenerationCompleteEvent += MapGenerationCompleteEvent;
    }

    private void MapGenerationCompleteEvent(MapGenerationCompleteEvent mapGenerationCompleteEvent)
    {
        MapGenerator.MapGenerationCompleteEvent -= MapGenerationCompleteEvent;
        //After the map is generated, create the player instance.
        //当地图生成完成后，创建玩家实例。
        if (GameSceneNodeHolder.Player != null)
        {
            //An existing player instance will not be created.
            //已经存在玩家实例，不再创建。
            GameSceneNodeHolder.Player.Position = GlobalPosition;
            return;
        }
        if (GameSceneNodeHolder.PlayerContainer == null)
        {
            return;
        }

        if (_playerPackedScene == null)
        {
            LogCat.LogError("player_packed_scene_not_exist");
            return;
        }

        var playerNode = _playerPackedScene.Instantiate();
        if (playerNode is not Player player)
        {
            return;
        }

        GameSceneNodeHolder.PlayerContainer.AddChild(player);
        GameSceneNodeHolder.Player = player;
        player.Position = GlobalPosition;
        LogCat.LogWithFormat("player_spawn_debug", player.ReadOnlyCharacterName, player.Position);
    }
}