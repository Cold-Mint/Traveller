﻿using System;
using System.Threading.Tasks;
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
public partial class PlayerSpawn : Marker2D, ISpawnMarker
{
    private PackedScene? _playerPackedScene;
    /// <summary>
    /// <para>The player's generated wave count</para>
    /// <para>玩家的生成波数</para>
    /// </summary>
    private const int PlayerWaveNumber = 1;

    public override void _Ready()
    {
        base._Ready();
        _playerPackedScene = ResourceLoader.Load<PackedScene>("res://prefab/entitys/Character.tscn");
        EventBus.MapGenerationCompleteEvent += MapGenerationCompleteEvent;
        EventBus.GameReplayEvent += GameReplayEvent;
    }

    private void GameReplayEvent(GameReplayEvent gameReplayEvent)
    {
        if (GameSceneDepend.Player == null)
        {
            LogCat.LogError("cannot_resurrect_non_existent_player");
            return;
        }
        GameSceneDepend.Player.GlobalPosition = GlobalPosition;
        GameSceneDepend.Player.FullHpRevive();
    }


    private async Task MapGenerationCompleteEvent(MapGenerationCompleteEvent mapGenerationCompleteEvent)
    {
        //After the map is generated, create the player instance.
        //当地图生成完成后，创建玩家实例。
        if (GameSceneDepend.Player != null)
        {
            //An existing player instance will not be created.
            //已经存在玩家实例，不再创建。
            return;
        }
        await Spawn(PlayerWaveNumber);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventBus.MapGenerationCompleteEvent -= MapGenerationCompleteEvent;
        EventBus.GameReplayEvent -= GameReplayEvent;
    }

    public async Task<Node2D[]?> Spawn(int waveNumber)
    {
        if (waveNumber != PlayerWaveNumber)
        {
            return null;
        }
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
        var itemContainer = GameSceneDepend.GameGuiTemplate?.HotBar?.GetItemContainer();
        if (itemContainer == null)
        {
            //Throws an exception when the item container is empty.
            //当物品容器为空时，抛出异常。
            throw new InvalidOperationException(TranslationServerUtils.Translate("log_item_container_is_null"));
        }

        playerNode.ItemContainer = itemContainer;
        GameSceneDepend.Player = playerNode;
        playerNode.GlobalPosition = GlobalPosition;
        return await Task.FromResult(new[]
        {
            playerNode
        });
    }

    public int GetMaxWaveNumber()
    {
        return PlayerWaveNumber;
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