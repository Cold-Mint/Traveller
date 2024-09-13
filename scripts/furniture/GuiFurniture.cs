using Godot;

namespace ColdMint.scripts.furniture;

/// <summary>
/// <para>GUIFurnitureTemplate</para>
/// <para>带有图形用户页面的家居模板</para>
/// </summary>
public partial class GuiFurniture : Furniture
{
    /// <summary>
    /// <para>Operating range of furniture</para>
    /// <para>家具的操作范围</para>
    /// </summary>
    /// <remarks>
    ///<para>For furniture with graphical user pages, the player must enter the action range and press the shortcut key to display the UI page.</para>
    ///<para>对于带有图形用户页面的家具来说，玩家必须进入操作范围内按下快捷键才能显示UI页面。</para>
    /// </remarks>
    private Area2D? _operateArea2D;

    public override void _Ready()
    {
        base._Ready();
        _operateArea2D = GetNode<Area2D>("OperateArea2D");
        _operateArea2D.BodyEntered += OnBodyEntered;
        _operateArea2D.BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node node)
    {
        
    }

    private void OnBodyExited(Node2D node2D)
    {
        
    }

    public override void _ExitTree()
    {
        if (_operateArea2D != null)
        {
            _operateArea2D.BodyEntered -= OnBodyEntered;
            _operateArea2D.BodyExited -= OnBodyExited;
        }
    }
}