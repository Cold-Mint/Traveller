using ColdMint.scripts.inventory;
using ColdMint.scripts.map.miniMap;
using Godot;

namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>Game Gui Template</para>
/// <para>游戏Gui模板</para>
/// </summary>
public partial class GameGuiTemplate : Control, IGameGui
{
    private Label? _fpsLabel;
    private Button? _recreateMapButton;
    private Label? _seedLabel;
    private MiniMap? _miniMap;
    protected HealthBarUi? ProtectedHealthBar;
    protected HotBar? ProtectedHotBar;
    private AnimationPlayer? _miniMapAnimationPlayer;
    public override void _Ready()
    {
        base._Ready();
        _fpsLabel = GetNode<Label>("FPSLabel");
        _recreateMapButton = GetNode<Button>("RecreateMapButton");
        _seedLabel = GetNode<Label>("SeedLabel");
        _miniMap = GetNode<MiniMap>("MapContainer/Control/MiniMap");
        _miniMapAnimationPlayer = GetNode<AnimationPlayer>("MapContainer/MiniMapAnimationPlayer");
    }

    public Label? FpsLabel
    {
        get => _fpsLabel;
    }
    public Button? RecreateMapButton
    {
        get => _recreateMapButton;
    }
    public Label? SeedLabel
    {
        get => _seedLabel;
    }
    public MiniMap? MiniMap
    {
        get => _miniMap;
    }
    public HealthBarUi? HealthBar
    {
        get => ProtectedHealthBar;
    }
    public HotBar? HotBar
    {
        get => ProtectedHotBar;
    }
    public AnimationPlayer? MiniMapAnimationPlayer
    {
        get => _miniMapAnimationPlayer;
    }
    public virtual TouchScreenButton? LeftButton
    {
        get => null;
    }
    public virtual TouchScreenButton? RightButton
    {
        get => null;
    }
    public virtual TouchScreenButton? JumpButton
    {
        get => null;
    }
    public virtual TouchScreenButton? PickButton
    {
        get => null;
    }
    public virtual RockerButton? ThrowButton
    {
        get => null;
    }
}