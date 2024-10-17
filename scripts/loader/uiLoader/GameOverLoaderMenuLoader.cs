using ColdMint.scripts.map.events;
using Godot;

namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>GameOverLoaderMenuLoader</para>
/// <para>游戏结束菜单加载器</para>
/// </summary>
public partial class GameOverLoaderMenuLoader : UiLoaderTemplate
{
    private Label? _deathInfoLabel;

    public override void InitializeUi()
    {
        Hide();
    }

    public override void InitializeData()
    {
        _deathInfoLabel =
            GetNode<Label>("CenterContainer/VBoxContainer/MarginContainer/CenterContainer2/DeathInfoLabel");
        EventBus.GameOverEvent += OnGameOver;
        EventBus.GameReplayEvent += OnGameReplayEvent;
    }

    private void OnGameReplayEvent(GameReplayEvent obj)
    {
        Hide();
    }

    public override void LoadUiActions()
    {
    }

    private void OnGameOver(GameOverEvent gameOverEvent)
    {
        if (_deathInfoLabel == null)
        {
            return;
        }

        Show();
        _deathInfoLabel.Text = gameOverEvent.DeathInfo;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        EventBus.GameOverEvent -= OnGameOver;
        EventBus.GameReplayEvent -= OnGameReplayEvent;
    }
}