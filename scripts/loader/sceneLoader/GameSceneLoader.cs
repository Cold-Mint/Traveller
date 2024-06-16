using System.Threading.Tasks;
using ColdMint.scripts.inventory;
using ColdMint.scripts.map;
using ColdMint.scripts.map.LayoutParsingStrategy;
using ColdMint.scripts.map.layoutStrategy;
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
        //Loading the blood bar scene
        //加载血条场景
        var healthBarUi = GetNode<HealthBarUi>("CanvasLayer/Control/VBoxContainer/HealthBarUi");
        GameSceneNodeHolder.HealthBarUi = healthBarUi;
        //Load HotBar
        //加载HotBar
        var hotBar = GetNode<HotBar>("CanvasLayer/Control/VBoxContainer/HotBar");
        GameSceneNodeHolder.HotBar = hotBar;
        //Backpack Ui container
        //背包Ui容器
        var backpackUiContainer = GetNode<Control>("CanvasLayer/BackpackUIContainer");
        GameSceneNodeHolder.BackpackUiContainer = backpackUiContainer;
        //Load operation prompt
        //加载操作提示
        var operationTip = GetNode<RichTextLabel>("CanvasLayer/Control/VBoxContainer/OperationTip");
        GameSceneNodeHolder.OperationTipLabel = operationTip;
        //Loaded weapon container
        //加载武器容器
        var weaponContainer = GetNode<Node2D>("WeaponContainer");
        GameSceneNodeHolder.WeaponContainer = weaponContainer;
        //Load Packsack container
        //加载背包容器
        var packsackContainer = GetNode<Node2D>("PacksackContainer");
        GameSceneNodeHolder.PacksackContainer = packsackContainer;
        //Load AICharacter container
        //加载AICharacter容器
        var aiCharacterContainer = GetNode<Node2D>("AICharacterContainer");
        GameSceneNodeHolder.AiCharacterContainer = aiCharacterContainer;
        //Load player container
        //加载玩家容器
        var playerContainer = GetNode<Node2D>("PlayerContainer");
        GameSceneNodeHolder.PlayerContainer = playerContainer;
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