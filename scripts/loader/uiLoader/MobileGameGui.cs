using ColdMint.scripts.inventory;
using Godot;

namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>MobileGameGui</para>
/// <para>移动平台Gui</para>
/// </summary>
public partial class MobileGameGui : GameGuiTemplate
{
    private TouchScreenButton? _leftButton;
    private TouchScreenButton? _rightButton;
    private TouchScreenButton? _jumpButton;
    private TouchScreenButton? _pickButton;
    private RockerButton? _throwButton;
    public override void _Ready()
    {
        base._Ready();
        ProtectedHealthBar = GetNode<HealthBarUi>("HealthBarUi");
        ProtectedHotBar = GetNode<HotBar>("HotBar");
        _leftButton = GetNode<TouchScreenButton>("MoveControl/LeftButton");
        _rightButton = GetNode<TouchScreenButton>("MoveControl/RightButton");
        _jumpButton = GetNode<TouchScreenButton>("ActionControl/JumpButton");
        _pickButton = GetNode<TouchScreenButton>("ActionControl/PickButton");
        _throwButton = GetNode<RockerButton>("ActionControl/ThrowButton");
    }


    public override TouchScreenButton? LeftButton
    {
        get => _leftButton;
    }
    public override TouchScreenButton? RightButton
    {
        get => _rightButton;
    }
    public override TouchScreenButton? JumpButton
    {
        get => _jumpButton;
    }


    public override TouchScreenButton? PickButton
    {
        get => _pickButton;
    }
    public override RockerButton? ThrowButton
    {
        get => _throwButton;
    }
}