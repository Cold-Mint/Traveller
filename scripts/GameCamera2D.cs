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

    private const float Speed = 500;
    private readonly Vector2 _zoomUnit = new(0.1f, 0.1f);

    private Vector2? _originalPosition;
    private Vector2? _originalZoom;

    /// <summary>
    /// <para>SetOriginalData</para>
    /// <para>设置原始数据</para>
    /// </summary>
    private void SetOriginalData()
    {
        _originalPosition = Position;
        _originalZoom = Zoom;
    }

    /// <summary>
    /// <para>Reset to raw data</para>
    /// <para>重置为原始数据</para>
    /// </summary>
    public void Reset()
    {
        Position = _originalPosition ?? Vector2.Zero;
        Zoom = _originalZoom ?? Vector2.One;
    }
    
    public override void _Ready()
    {
        base._Ready();
        SetOriginalData();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!FreeVision)
        {
            return;
        }

        var velocity = Vector2.Zero;
        var distance = Speed * (float)delta;
        var zoomFactor = Mathf.Max(Zoom.X, Zoom.Y);
        velocity.Y = distance * Input.GetAxis("ui_up", "ui_down") / zoomFactor;
        velocity.X = distance * Input.GetAxis("ui_left", "ui_right") / zoomFactor;
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