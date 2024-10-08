using System;
using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using ColdMint.scripts.map.events;
using ColdMint.scripts.map.room;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.map;

/// <summary>
/// <para>PlayerSpawn</para>
/// <para>玩家出生点</para>
/// </summary>
public partial class PlayerSpawn : Marker2D,ISpawnMarker
{
    private PackedScene? _playerPackedScene;

    public override void _Ready()
    {
        base._Ready();
        _playerPackedScene = GD.Load<PackedScene>("res://prefab/entitys/Character.tscn");
        EventBus.MapGenerationCompleteEvent += MapGenerationCompleteEvent;
        EventBus.GameReplayEvent += GameReplayEvent;
    }

    private void GameReplayEvent(GameReplayEvent gameReplayEvent)
    {
        if (GameSceneDepend.Player != null)
        {
            GameSceneDepend.Player.FullHpRevive();
            GameSceneDepend.Player.GlobalPosition = GlobalPosition;
            return;
        }
        Spawn();
    }
    

    private void MapGenerationCompleteEvent(MapGenerationCompleteEvent mapGenerationCompleteEvent)
    {
        //After the map is generated, create the player instance.
        //当地图生成完成后，创建玩家实例。
        if (GameSceneDepend.Player != null)
        {
            //An existing player instance will not be created.
            //已经存在玩家实例，不再创建。
            return;
        }

        Spawn();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventBus.MapGenerationCompleteEvent -= MapGenerationCompleteEvent;
        EventBus.GameReplayEvent -= GameReplayEvent;
    }

    public Node2D? Spawn()
    {
        if (GameSceneDepend.PlayerContainer == null)
        {
            return null;
        }

        if (_playerPackedScene == null)
        {
            LogCat.LogError("player_packed_scene_not_exist");
            return null;
        }

        var playerNode =
            NodeUtils.InstantiatePackedScene<Player>(_playerPackedScene);
        if (playerNode == null)
        {
            return null;
        }

        //The player's parent node must be GameSceneDepend PlayerContainer.
        //玩家的父节点必须是GameSceneDepend.PlayerContainer。
        NodeUtils.CallDeferredAddChild(GameSceneDepend.PlayerContainer, playerNode);
        var itemContainer = GameSceneDepend.HotBar?.GetItemContainer();
        if (itemContainer == null)
        {
            //Throws an exception when the item container is empty.
            //当物品容器为空时，抛出异常。
            throw new NullReferenceException(TranslationServerUtils.Translate("log_item_container_is_null"));
        }

        playerNode.ItemContainer = itemContainer;
        GameSceneDepend.Player = playerNode;
        playerNode.GlobalPosition = GlobalPosition;
        return playerNode;
    }

    public bool CanQueueFree()
    {
        return false;
    }

    public void DoQueueFree()
    {
        QueueFree();
    }
}