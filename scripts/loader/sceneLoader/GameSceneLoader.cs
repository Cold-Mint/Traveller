using System.Threading.Tasks;
using ColdMint.scripts.inventory;
using ColdMint.scripts.map;
using ColdMint.scripts.map.LayoutParsingStrategy;
using ColdMint.scripts.map.layoutStrategy;
using ColdMint.scripts.map.miniMap;
using ColdMint.scripts.map.RoomPlacer;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.loader.sceneLoader;

/// <summary>
/// <para>Game scene loader</para>
/// <para>游戏场景加载器</para>
/// </summary>
public partial class GameSceneLoader : SceneLoaderTemplate
{
    private Label? _seedLabel;

    public override Task InitializeData()
    {
        RenderingServer.SetDefaultClearColor(Color.FromHsv(0, 0, 0));
        //Loading the blood bar scene
        //加载血条场景
        var healthBarUi = GetNode<HealthBarUi>("CanvasLayer/Control/VBoxContainer/HealthBarUi");
        GameSceneDepend.HealthBarUi = healthBarUi;
        //Load HotBar
        //加载HotBar
        var hotBar = GetNode<HotBar>("CanvasLayer/Control/VBoxContainer/HotBar");
        GameSceneDepend.HotBar = hotBar;
        //Backpack Ui container
        //背包Ui容器
        var backpackUiContainer = GetNode<UiGroup>("CanvasLayer/DynamicUiGroup");
        GameSceneDepend.DynamicUiGroup = backpackUiContainer;
        //Loaded weapon container
        //加载武器容器
        var weaponContainer = GetNode<Node2D>("WeaponContainer");
        GameSceneDepend.WeaponContainer = weaponContainer;
        //Load projectile container
        //加载抛射体容器
        var projectileContainer = GetNode<Node2D>("ProjectileContainer");
        GameSceneDepend.ProjectileContainer = projectileContainer;
        //Load magic container
        //加载魔术容器
        var magicContainer = GetNode<Node2D>("SpellContainer");
        GameSceneDepend.SpellContainer = magicContainer;
        //Load Packsack container
        //加载背包容器
        var packsackContainer = GetNode<Node2D>("PacksackContainer");
        GameSceneDepend.PacksackContainer = packsackContainer;
        //Load AICharacter container
        //加载AICharacter容器
        var aiCharacterContainer = GetNode<Node2D>("AICharacterContainer");
        GameSceneDepend.AiCharacterContainer = aiCharacterContainer;
        //Load player container
        //加载玩家容器
        var playerContainer = GetNode<Node2D>("PlayerContainer");
        GameSceneDepend.PlayerContainer = playerContainer;
        //Load the pickable container
        //加载可拾捡物容器
        var pickAbleContainer = GetNode<Node2D>("PickAbleContainer");
        GameSceneDepend.PickAbleContainer = pickAbleContainer;
        //Setting up the mini map
        //设置迷你地图
        var miniMap = GetNode<MiniMap>("CanvasLayer/Control/MapContainer/MiniMap");
        GameSceneDepend.MiniMap = miniMap;
        return Task.CompletedTask;
    }

    public override async Task LoadScene()
    {
        var debugMode = Config.IsDebug();
        var recreateMapButton = GetNodeOrNull<Button>("CanvasLayer/Control/RecreateMapButton");
        if (recreateMapButton != null)
        {
            recreateMapButton.Visible = debugMode;
            recreateMapButton.Pressed += () => { _ = GenerateMap(); };
        }

        _seedLabel = GetNodeOrNull<Label>("CanvasLayer/Control/SeedLabel");
        if (_seedLabel != null)
        {
            _seedLabel.Visible = debugMode;
        }

        MapGenerator.MapRoot = GetNode<Node>("MapRoot");
        MapGenerator.LayoutStrategy = new TestLayoutStrategy();
        MapGenerator.LayoutParsingStrategy = new SequenceLayoutParsingStrategy();
        MapGenerator.RoomPlacementStrategy = new PatchworkRoomPlacementStrategy();
        await GenerateMap();
    }

    /// <summary>
    /// <para>Generate map</para>
    /// <para>生成地图</para>
    /// </summary>
    private async Task GenerateMap()
    {
        MapGenerator.Seed = GuidUtils.GetGuid();
        if (_seedLabel != null)
        {
            //If you have a seedLabel, then set the seed to it.
            //如果有seedLabel，那么将种子设置上去。
            var seedInfo = TranslationServerUtils.TranslateWithFormat("ui_seed_info", MapGenerator.Seed);
            _seedLabel.Text = seedInfo ?? $"Seed: {MapGenerator.Seed}";
        }

        await MapGenerator.GenerateMap();
    }
}