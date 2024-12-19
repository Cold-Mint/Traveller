using Godot;

namespace ColdMint.scripts;

/// <summary>
/// <para>In-game 2D camera</para>
/// <para>游戏内的2D相机</para>
/// </summary>
public partial class GameCamera2D : Camera2D
{
    /// <summary>
    /// <para>FreeVision</para>
    /// <para>自由视野模式</para>
    /// </summary>
    public bool FreeVision;

    private float _speed = 500;
    private Vector2 _zoomUnit = new(0.1f, 0.1f);


    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!FreeVision)
        {
            return;
        }

        var velocity = Vector2.Zero;
        var distance = _speed * (float)delta;
        velocity.Y = distance * Input.GetAxis("ui_up", "ui_down");
        var horizontallyAxis = Input.GetAxis("ui_left", "ui_right");
        velocity.X = distance * horizontallyAxis;
        Position += velocity;
        if (Input.IsActionJustPressed("hotbar_next") && Zoom > _zoomUnit)
        {
            Zoom -= _zoomUnit;
        }

        if (Input.IsActionJustPressed("hotbar_previous"))
        {
            Zoom += _zoomUnit;
        }
    }
}