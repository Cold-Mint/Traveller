using Godot;

namespace ColdMint.scripts;

/// <summary>
/// <para>FloatLabel</para>
/// <para>悬浮标签</para>
/// </summary>
/// <remarks>
///<para>When the player mouses over a creature or item, use this tab to follow the mouse movement to display information.</para>
///<para>当玩家鼠标指向某个生物或者物品时，使用此标签跟随鼠标移动显示信息。</para>
/// </remarks>
public partial class FloatLabel : Label
{
    public bool Follow;

    private Vector2 _offset = new Vector2(8, 8);

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Follow)
        {
            GlobalPosition = _offset + GetGlobalMousePosition();
        }
    }
}