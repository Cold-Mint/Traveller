using ColdMint.scripts.debug;
using ColdMint.scripts.map.dateBean;
using ColdMint.scripts.map.events;
using ColdMint.scripts.map.preview;
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
    private Vector2 _miniMapMidpointCoordinate;

    /// <summary>
    /// <para>The midpoint coordinates of the mini map</para>
    /// <para>迷你地图的中点坐标</para>
    /// </summary>
    public Vector2 MiniMapMidpointCoordinate => _miniMapMidpointCoordinate;

    /// <summary>
    /// <para>The master node of the map</para>
    /// <para>地图的主人节点</para>
    /// </summary>
    public Node2D? OwnerNode { get; set; }

    public override void _Ready()
    {
        _roomPreviewContainer = GetNode<Node2D>("RoomPreviewContainer");
        _miniMapMidpointCoordinate = Size / 2;
        EventBus.MapGenerationPlaceRoomFinishEvent += MapGenerationPlaceRoomFinishEvent;
    }

    /// <summary>
    /// <para>After the map generator completes placing the room</para>
    /// <para>地图生成器放置房间完成后</para>
    /// </summary>
    /// <param name="mapGenerationPlaceRoomFinishEvent"></param>
    private void MapGenerationPlaceRoomFinishEvent(MapGenerationPlaceRoomFinishEvent mapGenerationPlaceRoomFinishEvent)
    {
        var roomPlacementData = mapGenerationPlaceRoomFinishEvent.RoomPlacementData;
        if (roomPlacementData?.NewRoom == null || mapGenerationPlaceRoomFinishEvent.RoomNodeDataId == null)
        {
            return;
        }

        var tileMapLayer = roomPlacementData.NewRoom.GetTileMapLayer(Config.TileMapLayerName.Ground);
        if (!CreateRoomPreview(tileMapLayer,
                CalculateRelativePositionOnTheMinimap(tileMapLayer, roomPlacementData)))
        {
            LogCat.LogWithFormat("failed_to_create_room_preview", LogCat.LogLabel.Default, LogCat.UploadFormat,
                mapGenerationPlaceRoomFinishEvent.RoomNodeDataId);
        }
    }


    /// <summary>
    /// <para>CalculateRelativePositionOnTheMinimap</para>
    /// <para>计算在迷你地图上的相对位置</para>
    /// </summary>
    /// <returns>
    ///<para>Returns the position relative to the point in the minimap container</para>
    ///<para>返回相对对于迷你地图容器中点的位置</para>
    /// </returns>
    private Vector2? CalculateRelativePositionOnTheMinimap(TileMapLayer? groundTileMapLayer,
        RoomPlacementData roomPlacementData)
    {
        if (groundTileMapLayer == null || roomPlacementData.Position == null)
        {
            return null;
        }

        return roomPlacementData.Position.Value / Config.CellSize * Config.RoomPreviewScale;
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
    private bool CreateRoomPreview(TileMapLayer? groundTileMapLayer, Vector2? position)
    {
        if (GameSceneDepend.MiniMapContainerNode == null || position == null)
        {
            return false;
        }

        var image = RoomPreview.CreateImage(groundTileMapLayer);
        if (image == null)
        {
            return false;
        }

        var sprite = new TextureRect();
        sprite.Scale = new Vector2(Config.RoomPreviewScale, Config.RoomPreviewScale);
        sprite.Texture = image;
        if (GameSceneDepend.MiniMap != null)
        {
            sprite.Position = GameSceneDepend.MiniMap.MiniMapMidpointCoordinate + position.Value;
        }

        NodeUtils.CallDeferredAddChild(GameSceneDepend.MiniMapContainerNode, sprite);
        return true;
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
        EventBus.MapGenerationPlaceRoomFinishEvent -= MapGenerationPlaceRoomFinishEvent;
    }
}