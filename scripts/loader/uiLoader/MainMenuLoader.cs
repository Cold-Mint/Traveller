using System;
using System.IO;
using System.Text;
using ColdMint.scripts.camp;
using ColdMint.scripts.contribute;
using ColdMint.scripts.deathInfo;
using ColdMint.scripts.debug;
using ColdMint.scripts.inventory;
using ColdMint.scripts.loot;
using ColdMint.scripts.map;
using ColdMint.scripts.map.roomInjectionProcessor;
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
        if (Config.IsDebug())
        {
            //Set the minimum log level to Info in debug mode.(Print all logs)
            //在调试模式下将最小日志等级设置为Info。（打印全部日志）
            LogCat.MinLogLevel = LogCat.InfoLogLevel;
        }
        else
        {
            //Disable all logs in the release version.
            //在发行版禁用所有日志。
            LogCat.MinLogLevel = LogCat.DisableAllLogLevel;
        }
        ContributorDataManager.RegisterAllContributorData();
        DeathInfoGenerator.RegisterDeathInfoHandler(new SelfDeathInfoHandler());
        MapGenerator.RegisterRoomInjectionProcessor(new ChanceRoomInjectionProcessor());
        MapGenerator.RegisterRoomInjectionProcessor(new TimeIntervalRoomInjectorProcessor());
        //Register the corresponding encoding provider to solve the problem of garbled Chinese path of the compressed package
        //注册对应的编码提供程序，解决压缩包中文路径乱码问题
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //创建游戏数据文件夹
        var dataPath = Config.GetGameDataDirectory();
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }


        //Registered camp
        //注册阵营
        var defaultCamp = new Camp(Config.CampId.Default)
        {
            FriendInjury = true
        };
        CampManager.SetDefaultCamp(defaultCamp);
        var mazoku = new Camp(Config.CampId.Mazoku);
        CampManager.AddCamp(mazoku);
        var aborigines = new Camp(Config.CampId.Aborigines);
        CampManager.AddCamp(aborigines);
        _gameScene = GD.Load<PackedScene>("res://scenes/game.tscn");
        _contributor = GD.Load<PackedScene>("res://scenes/contributor.tscn");
        _levelGraphEditor = GD.Load<PackedScene>("res://scenes/levelGraphEditor.tscn");

        //Register ItemTypes from file
        //从文件注册物品类型
        ItemTypeRegister.RegisterFromFile();
        //Hardcoded ItemTypes Register
        //硬编码注册物品类型
        ItemTypeRegister.StaticRegister();

        //静态注册掉落表
        LootRegister.StaticRegister();
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