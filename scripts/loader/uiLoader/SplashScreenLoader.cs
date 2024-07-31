using System.IO;
using System.Text;
using System.Threading.Tasks;
using ColdMint.scripts.camp;
using ColdMint.scripts.contribute;
using ColdMint.scripts.deathInfo;
using ColdMint.scripts.debug;
using ColdMint.scripts.inventory;
using ColdMint.scripts.loot;
using ColdMint.scripts.map;
using ColdMint.scripts.map.roomInjectionProcessor;
using ColdMint.scripts.mod;
using Godot;

namespace ColdMint.scripts.loader.uiLoader;

/// <summary>
/// <para>SplashScreenLoader</para>
/// <para>启动画面加载器</para>
/// </summary>
public partial class SplashScreenLoader : UiLoaderTemplate
{
    private Label? _loadingLabel;
    private PackedScene? _mainMenuScene;
    private AnimationPlayer? _animationPlayer;
    private string _startup = "startup";
    private Label? _nameLabel;

    public override void InitializeData()
    {
        _mainMenuScene = GD.Load<PackedScene>("res://scenes/mainMenu.tscn");
    }

    public override void InitializeUi()
    {
        _nameLabel = GetNode<Label>("NameLabel");
        _loadingLabel = GetNode<Label>("loadingStateLabel");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        //Disable animation in Debug mode.
        //在Debug模式禁用动画。
        if (Config.IsDebug())
        {
            _loadingLabel.Modulate = Colors.White;
            _nameLabel.Modulate = Colors.White;
            AnimationFinished(_startup);
        }
        else
        {
            _animationPlayer.Play(_startup);
            _animationPlayer.AnimationFinished += AnimationFinished;
        }
    }

    private async void AnimationFinished(StringName name)
    {
        await LoadingGlobalData();
        if (_mainMenuScene == null)
        {
            return;
        }

        GetTree().ChangeSceneToPacked(_mainMenuScene);
    }

    /// <summary>
    /// <para>Load the game's global data</para>
    /// <para>加载游戏的全局数据</para>
    /// </summary>
    private async Task LoadingGlobalData()
    {
        //Loading App configuration
        //加载App配置
        var appConfigData = AppConfig.LoadFromFile();
        if (appConfigData != null)
        {
            AppConfig.ApplyAppConfig(appConfigData);
        }

        //Set the minimum log level to Info in debug mode.(Print all logs)
        //在调试模式下将最小日志等级设置为Info。（打印全部日志）
        //Disable all logs in the release version.
        //在发行版禁用所有日志。
        LogCat.MinLogLevel = Config.IsDebug() ? LogCat.InfoLogLevel : LogCat.DisableAllLogLevel;
        ContributorDataManager.RegisterAllContributorData();
        DeathInfoGenerator.RegisterDeathInfoHandler(new SelfDeathInfoHandler());
        MapGenerator.RegisterRoomInjectionProcessor(new ChanceRoomInjectionProcessor());
        MapGenerator.RegisterRoomInjectionProcessor(new TimeIntervalRoomInjectorProcessor());
        //Register the corresponding encoding provider to solve the problem of garbled Chinese path of the compressed package
        //注册对应的编码提供程序，解决压缩包中文路径乱码问题
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //Create a game data folder
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
        //Register ItemTypes from file
        //从文件注册物品类型
        ItemTypeRegister.RegisterFromFile();
        //Hardcoded ItemTypes Register
        //硬编码注册物品类型
        ItemTypeRegister.StaticRegister();
        //静态注册掉落表
        LootRegister.StaticRegister();
        //Load mod
        //加载模组
        if (Config.EnableMod())
        {
            var modPath = Config.GetModDataDirectory();
            if (!Directory.Exists(modPath))
            {
                Directory.CreateDirectory(modPath);
            }

            ModLoader.Init();
            ModLoader.LoadAllMods(modPath);
        }

        await Task.Yield();
    }
}