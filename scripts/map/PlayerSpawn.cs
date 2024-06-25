using System;
using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using ColdMint.scripts.map.events;
using ColdMint.scripts.utils;
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
            GameSceneNodeHolder.Player.ProcessMode = ProcessModeEnum.Inherit;
            GameSceneNodeHolder.Player.GlobalPosition = GlobalPosition;
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

        var playerNode =
            NodeUtils.InstantiatePackedScene<Player>(_playerPackedScene);
        if (playerNode == null)
        {
            return;
        }

        //The player's parent node must be GameSceneNodeHolder PlayerContainer.
        //玩家的父节点必须是GameSceneNodeHolder.PlayerContainer。
        NodeUtils.CallDeferredAddChild(GameSceneNodeHolder.PlayerContainer, playerNode);
        var itemContainer = GameSceneNodeHolder.HotBar?.GetItemContainer();
        if (itemContainer == null)
        {
            //Throws an exception when the item container is empty.
            //当物品容器为空时，抛出异常。
            throw new NullReferenceException(TranslationServerUtils.Translate("log_item_container_is_null"));
        }

        playerNode.ItemContainer = itemContainer;
        GameSceneNodeHolder.Player = playerNode;
        playerNode.GlobalPosition = GlobalPosition;
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