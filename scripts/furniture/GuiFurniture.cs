using ColdMint.scripts.character;
using ColdMint.scripts.loader.uiLoader;
using ColdMint.scripts.utils;
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

    /// <summary>
    /// <para>Whether the player is within range of the operation</para>
    /// <para>玩家是否在操作范围内</para>
    /// </summary>
    private bool _playerInRange;
    [Export]
    public string? Path;

    /// <summary>
    /// <para>There's a mouse hover</para>
    /// <para>有鼠标悬停</para>
    /// </summary>
    private bool _hasMouseOver;

    public override void _Ready()
    {
        base._Ready();
        _operateArea2D = GetNode<Area2D>("OperateArea2D");
        _operateArea2D.BodyEntered += OnBodyEntered;
        _operateArea2D.BodyExited += OnBodyExited;
        _operateArea2D.SetCollisionMaskValue(Config.LayerNumber.Player, true);
        if (Path != null)
        {
            GameSceneDepend.DynamicUiGroup?.RegisterControl(Path, () =>
            {
                var packedScene = ResourceLoader.Load<PackedScene>(Path);
                return NodeUtils.InstantiatePackedScene<SpellEditorUi>(packedScene);
            });
        }
    }

    /// <summary>
    /// <para>Use furniture</para>
    /// <para>使用家具</para>
    /// </summary>
    /// <param name="player"></param>
    private void Use(Player player)
    {
        if (Path == null)
        {
            return;
        }
        GameSceneDepend.DynamicUiGroup?.ShowControl(Path);
    }

    public override void _MouseEnter()
    {
        base._MouseEnter();
        _hasMouseOver = true;
        GameSceneDepend.IsMouseOverFurnitureGui = true;
    }

    public override void _MouseExit()
    {
        base._MouseExit();
        _hasMouseOver = false;
        GameSceneDepend.IsMouseOverFurnitureGui = false;
    }

    private void OnBodyEntered(Node node)
    {
        if (node is Player)
        {
            _playerInRange = true;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (GameSceneDepend.Player == null || !_playerInRange || !_hasMouseOver)
        {
            return;
        }
        if (Input.IsActionJustPressed("use_item"))
        {
            Use(GameSceneDepend.Player);
        }
    }

    private void OnBodyExited(Node2D node2D)
    {
        if (node2D is Player)
        {
            _playerInRange = false;
            if (Path != null)
            {
                GameSceneDepend.DynamicUiGroup?.HideControl(Path);
            }
        }
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