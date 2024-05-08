using Godot;

namespace ColdMint.scripts.map;

public partial class RoomAreaDetector : Node2D
{
    private Area2D? _area2D;

    public override void _Ready()
    {
        base._Ready();
        _area2D = GetNode<Area2D>("Area2D");
    }
    
}