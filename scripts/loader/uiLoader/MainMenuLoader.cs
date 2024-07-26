using System;
using System.Text;
using ColdMint.scripts.contribute;
using ColdMint.scripts.debug;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>UI loader for the main menu</para>
/// <para>主菜单的UI加载器</para>
/// </summary>
public partial class MainMenuLoader : UiLoaderTemplate
{
    private Button? _startGameButton;
    private Label? _copyrightLabel;
    private StringBuilder? _copyrightBuilder;
    private PackedScene? _gameScene;
    private PackedScene? _contributor;
    private PackedScene? _levelGraphEditor;
    private Label? _sloganLabel;
    private Label? _versionLabel;
    private Button? _levelGraphEditorButton;
    private LinkButton? _contributorButton;

    public override void InitializeData()
    {
        _gameScene = GD.Load<PackedScene>("res://scenes/game.tscn");
        _contributor = GD.Load<PackedScene>("res://scenes/contributor.tscn");
        _levelGraphEditor = GD.Load<PackedScene>("res://scenes/levelGraphEditor.tscn");
    }

    public override void InitializeUi()
    {
        _contributorButton = GetNode<LinkButton>("VBoxContainer2/ContributorButton");
        _startGameButton = GetNode<Button>("StartGameButton");
        _levelGraphEditorButton = GetNode<Button>("levelGraphEditorButton");
        //The level map editor is only available in debug mode.
        //关卡图编辑器仅在调试模式可用。
        _levelGraphEditorButton.Visible = Config.IsDebug();
        _startGameButton.GrabFocus();
        _versionLabel = GetNode<Label>("VBoxContainer2/VersionLabel");
        //Generative copyright
        //生成版权
        _copyrightLabel = GetNode<Label>("VBoxContainer/CopyrightLabel");
        _sloganLabel = GetNode<Label>("CenterContainer2/SloganLabel");
        _copyrightBuilder = new StringBuilder();
        _copyrightBuilder.Append('\u00a9');
        var currentYear = DateTime.Now.Year;
        _copyrightBuilder.Append(Config.CreationYear);
        if (currentYear != Config.CreationYear)
        {
            _copyrightBuilder.Append('-');
            _copyrightBuilder.Append(currentYear);
        }

        _copyrightBuilder.Append(' ');
        _copyrightBuilder.Append(Config.CompanyName);
        _copyrightBuilder.Append(" all rights reserved.");
        _copyrightLabel.Text = _copyrightBuilder.ToString();
        _versionLabel.Text = "ver." + Config.GetVersion();
        _sloganLabel.Text = SloganProvider.GetSlogan();
        _contributorButton.Text =
            TranslationServerUtils.TranslateWithFormat("ui_contributor_tips",
                ContributorDataManager.GetContributorTotals());
    }

    public override void LoadUiActions()
    {
        if (_startGameButton != null)
        {
            _startGameButton.Pressed += () =>
            {
                if (_gameScene == null)
                {
                    return;
                }

                GetTree().ChangeSceneToPacked(_gameScene);
            };
        }

        if (_contributorButton != null)
        {
            _contributorButton.Pressed += () =>
            {
                if (_contributor == null)
                {
                    return;
                }

                GetTree().ChangeSceneToPacked(_contributor);
            };
        }

        if (_levelGraphEditorButton != null)
        {
            _levelGraphEditorButton.Pressed += () =>
            {
                LogCat.Log("level_graph_editor");
                if (_levelGraphEditor == null)
                {
                    return;
                }

                GetTree().ChangeSceneToPacked(_levelGraphEditor);
            };
        }
    }
}
