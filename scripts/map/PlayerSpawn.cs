using ColdMint.scripts.character;
using ColdMint.scripts.debug;
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
        EventManager.MapGenerationCompleteEvent += MapGenerationCompleteEvent;
        EventManager.GameReplayEvent += GameReplayEvent;
    }

    private void GameReplayEvent(GameReplayEvent gameReplayEvent)
    {
        if (GameSceneNodeHolder.Player != null)
        {
            GameSceneNodeHolder.Player.Position = GlobalPosition;
            GameSceneNodeHolder.Player.Revive(GameSceneNodeHolder.Player.MaxHp);
            return;
        }
        SpawnPlayer();
    }

    /// <summary>
    /// <para>Generate player instance</para>
    /// <para>生成玩家实例</para>
    /// </summary>
    private void SpawnPlayer()
    {
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

        player.ItemContainer = GameSceneNodeHolder.HotBar;
        GameSceneNodeHolder.PlayerContainer.AddChild(player);
        GameSceneNodeHolder.Player = player;
        player.Position = GlobalPosition;
        LogCat.LogWithFormat("player_spawn_debug", player.ReadOnlyCharacterName, player.Position);
    }

    private void MapGenerationCompleteEvent(MapGenerationCompleteEvent mapGenerationCompleteEvent)
    {
        //After the map is generated, create the player instance.
        //当地图生成完成后，创建玩家实例。
        if (GameSceneNodeHolder.Player != null)
        {
            //An existing player instance will not be created.
            //已经存在玩家实例，不再创建。
            return;
        }

        SpawnPlayer();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventManager.MapGenerationCompleteEvent -= MapGenerationCompleteEvent;
        EventManager.GameReplayEvent -= GameReplayEvent;
    }
}