using ColdMint.scripts.pool;
using Godot;

namespace ColdMint.scripts.barrage;

/// <summary>
/// <para>BarrageLabel</para>
/// <para>弹幕标签</para>
/// </summary>
public partial class BarrageLabel : RichTextLabel, IPoolable
{
    [Export] private VisibleOnScreenNotifier2D? _visibleOnScreenNotifier2D;

    private bool _recycle;
    private readonly Vector2 _barrageVelocity = new(-Config.BarrageSpeed, 0);

    public override void _Ready()
    {
        base._Ready();
        if (_visibleOnScreenNotifier2D == null)
        {
            return;
        }
        _visibleOnScreenNotifier2D.ScreenEntered += VisibleOnScreenNotifier2DOnScreenEntered;
        _visibleOnScreenNotifier2D.ScreenExited += VisibleOnScreenNotifier2DOnScreenExited;
        UpdateRectangle();
    }

    /// <summary>
    /// <para>SetLabelText</para>
    /// <para>设置标签的文本</para>
    /// </summary>
    /// <param name="text">
    ///<para>text</para>
    ///<para>文本</para>
    /// </param>
    public void SetLabelText(string text)
    {
        Text = text;
        UpdateRectangle();
    }


    /// <summary>
    /// <para>Updates the rectangle used to detect whether it is inside the screen</para>
    /// <para>更新用于检测是否在屏幕内的矩形</para>
    /// </summary>
    private void UpdateRectangle()
    {
        if (_visibleOnScreenNotifier2D != null)
        {
            _visibleOnScreenNotifier2D.Rect = new Rect2(0, 0, GetContentWidth(), GetContentHeight());
        }
    }

    private void VisibleOnScreenNotifier2DOnScreenExited()
    {
        _recycle = true;
        Visible = false;
    }

    private void VisibleOnScreenNotifier2DOnScreenEntered()
    {
        _recycle = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_recycle)
        {
            return;
        }

        Position += _barrageVelocity * (float)delta;
    }

    public bool CanRecycle()
    {
        return _recycle;
    }

    public void OnRecycle()
    {
        Visible = true;
    }
}