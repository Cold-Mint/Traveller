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
}