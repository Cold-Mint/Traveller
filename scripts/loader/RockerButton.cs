using Godot;

namespace ColdMint.scripts.loader;

/// <summary>
/// <para>Rocker button</para>
/// <para>摇杆按钮</para>
/// </summary>
public partial class RockerButton : TouchScreenButton
{
    
    private Vector2 _touchStartPos;
    
    public override void _Ready()
    {
        base._Ready();
        Pressed+= OnPressed;
        Released+= OnReleased;
    }

    /// <summary>
    /// <para>Gets the offset coordinates that the user pressed</para>
    /// <para>获取用户按下的偏移坐标</para>
    /// </summary>
    /// <returns>
    ///<para>The normalized vector</para>
    ///<para>归一化后的向量</para>
    /// </returns>
    public Vector2 GetOffSetPosition()
    {
       return _touchStartPos.DirectionTo(GetLocalMousePosition());
    }
    
    private void OnReleased()
    {
        _touchStartPos = Vector2.Zero;
    }

    private void OnPressed()
    {
        _touchStartPos = GetLocalMousePosition();
    }
    
}