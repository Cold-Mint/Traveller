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
    private Button? _restartButton;

    public override void InitializeUi()
    {
        Hide();
    }

    public override void InitializeData()
    {
        _restartButton = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/MarginContainer2/RestartButton");
        _deathInfoLabel =
            GetNode<Label>("CenterContainer/VBoxContainer/MarginContainer/CenterContainer2/DeathInfoLabel");
        EventManager.GameOverEvent += OnGameOver;
    }

    public override void LoadUiActions()
    {
        if (_restartButton != null)
        {
            _restartButton.Pressed += () =>
            {
                var replayEvent = new GameReplayEvent();
                EventManager.GameReplayEvent?.Invoke(replayEvent);
                Hide();
            };
        }
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
        EventManager.GameOverEvent -= OnGameOver;
    }
}
