using System.Collections.Generic;
using System.Threading.Tasks;
using ColdMint.scripts.debug;
using ColdMint.scripts.map.events;
using ColdMint.scripts.map.preview;
using ColdMint.scripts.map.room;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.map.miniMap;

/// <summary>
/// <para>Mini Map</para>
/// <para>迷你地图</para>
/// </summary>
public partial class MiniMap : NinePatchRect
{
    private Node2D? _roomPreviewContainer;

    /// <summary>
    /// <para>The midpoint coordinates of the mini map</para>
    /// <para>迷你地图的中点坐标</para>
    /// </summary>
    private Vector2 _miniMapMidpointCoordinate;

    /// <summary>
    /// <para>Mapping of rooms and room preview images</para>
    /// <para>房间和房间预览图的映射</para>
    /// </summary>
    private readonly Dictionary<Room, TextureRect> _roomToRoomPreviews = [];

    /// <summary>
    /// <para>The master node of the map</para>
    /// <para>地图的主人节点</para>
    /// </summary>
    public Node2D? OwnerNode { get; set; }

    public override void _Ready()
    {
        _roomPreviewContainer = GetNode<Node2D>("RoomPreviewContainer");
        _miniMapMidpointCoordinate = Size / 2;
        EventBus.MapGenerationCompleteEvent += MapGenerationCompleteEvent;
        EventBus.MapGenerationStartEvent += MapGenerationStartEvent;
    }

    /// <summary>
    /// <para>Clean up all room preview images in the mini map</para>
    /// <para>清理迷你地图内的所有房间预览图</para>
    /// </summary>
    private void Clear()
    {
        _roomToRoomPreviews.Clear();
        if (_roomPreviewContainer != null)
        {
            NodeUtils.DeleteAllChild(_roomPreviewContainer);
        }
    }

    private void MapGenerationStartEvent(MapGenerationStartEvent mapGenerationStartEvent)
    {
        Clear();
    }

    /// <summary>
    /// <para>Display room preview image</para>
    /// <para>显示房间预览图</para>
    /// </summary>
    /// <param name="room"></param>
    public void ShowRoomPreview(Room room)
    {
        if (_roomToRoomPreviews.TryGetValue(room, out var roomPreview))
        {
            roomPreview.Show();
        }
    }

    /// <summary>
    /// <para>After the map generator completes placing the room</para>
    /// <para>地图生成器放置房间完成后</para>
    /// </summary>
    /// <param name="mapGenerationCompleteEvent"></param>
    private async Task MapGenerationCompleteEvent(MapGenerationCompleteEvent mapGenerationCompleteEvent)
    {
        if (mapGenerationCompleteEvent.RoomDictionary == null)
        {
            return;
        }

        foreach (var dictionaryKey in mapGenerationCompleteEvent.RoomDictionary.Keys)
        {
            var roomDictionaryValue = mapGenerationCompleteEvent.RoomDictionary[dictionaryKey];
            var tileMapLayer = roomDictionaryValue.GetTileMapLayer(Config.TileMapLayerName.Ground);
            var textureRect = CreateRoomPreview(tileMapLayer,
                CalculateRelativePositionOnTheMinimap(roomDictionaryValue));
            if (textureRect == null)
            {
                LogCat.LogErrorWithFormat("failed_to_create_room_preview", LogCat.LogLabel.Default,
                    dictionaryKey);
            }
            else
            {
                _roomToRoomPreviews[roomDictionaryValue] = textureRect;
            }
        }
        await Task.CompletedTask;
    }


    /// <summary>
    /// <para>CalculateRelativePositionOnTheMinimap</para>
    /// <para>计算在迷你地图上的相对位置</para>
    /// </summary>
    /// <returns>
    ///<para>Returns the position relative to the point in the minimap container</para>
    ///<para>返回相对对于迷你地图容器中点的位置</para>
    /// </returns>
    private Vector2? CalculateRelativePositionOnTheMinimap(Room room)
    {
        if (room.RootNode == null)
        {
            return null;
        }

        return room.RootNode.Position / Config.CellSize * Config.RoomPreviewScale;
    }

    /// <summary>
    /// <para>Create a room preview image.</para>
    /// <para>创建房间预览图</para>
    /// </summary>
    /// <param name="groundTileMapLayer">
    ///<para>Layers that need to be drawn onto a minimap</para>
    ///<para>需要绘制到迷你地图上的图层</para>
    /// </param>
    /// <param name="position">
    ///<para>Relative to the position of the point in the minimap container</para>
    ///<para>相对于迷你地图容器中点的位置</para>
    /// </param>
    /// <returns></returns>
    private TextureRect? CreateRoomPreview(TileMapLayer? groundTileMapLayer, Vector2? position)
    {
        if (_roomPreviewContainer == null || position == null)
        {
            return null;
        }

        var image = RoomPreview.CreateImage(groundTileMapLayer);
        if (image == null)
        {
            return null;
        }

        var textureRect = new TextureRect
        {
            Scale = new Vector2(Config.RoomPreviewScale, Config.RoomPreviewScale),
            Texture = image,
            Position = _miniMapMidpointCoordinate + position.Value
        };
        textureRect.Hide();
        NodeUtils.CallDeferredAddChild(_roomPreviewContainer, textureRect);
        return textureRect;
    }

    public override void _Process(double delta)
    {
        if (_roomPreviewContainer == null)
        {
            return;
        }

        if (OwnerNode != null)
        {
            _roomPreviewContainer.Position = -OwnerNode.GlobalPosition / Config.CellSize * Config.RoomPreviewScale;
        }
    }

    public override void _ExitTree()
    {
        EventBus.MapGenerationCompleteEvent -= MapGenerationCompleteEvent;
        EventBus.MapGenerationStartEvent -= MapGenerationStartEvent;
    }
}