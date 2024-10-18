using ColdMint.scripts.inventory;

namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>Desktop Game Gui</para>
/// <para>桌面的Gui</para>
/// </summary>
public partial class DesktopGameGui : GameGuiTemplate
{
    public override void _Ready()
    {
        base._Ready();
        ProtectedHealthBar = GetNode<HealthBarUi>("VBoxContainer/HealthBarUi");
        ProtectedHotBar = GetNode<HotBar>("VBoxContainer/HotBar");
    }
}