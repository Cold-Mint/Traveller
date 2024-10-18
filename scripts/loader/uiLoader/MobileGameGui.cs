using ColdMint.scripts.inventory;
using Godot;

namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>MobileGameGui</para>
/// <para>移动平台Gui</para>
/// </summary>
public partial class MobileGameGui : GameGuiTemplate
{
    private TextureButton? _leftButton;
    private TextureButton? _rightButton;
    private TextureButton? _jumpButton;
    private TextureButton? _pickButton;
    private TextureButton? _throwButton;
    public override void _Ready()
    {
        base._Ready();
        ProtectedHealthBar = GetNode<HealthBarUi>("HealthBarUi");
        ProtectedHotBar = GetNode<HotBar>("HotBar");
        _leftButton = GetNode<TextureButton>("LeftButton");
        _rightButton = GetNode<TextureButton>("RightButton");
        _jumpButton = GetNode<TextureButton>("JumpButton");
        _pickButton = GetNode<TextureButton>("PickButton");
        _throwButton = GetNode<TextureButton>("ThrowButton");
    }


    public override TextureButton? LeftButton
    {
        get => _leftButton;
    }
    public override TextureButton? RightButton
    {
        get => _rightButton;
    }
    public override TextureButton? JumpButton
    {
        get => _jumpButton;
    }


    public override TextureButton? PickButton
    {
        get => _pickButton;
    }
    public override TextureButton? ThrowButton
    {
        get => _throwButton;
    }
}