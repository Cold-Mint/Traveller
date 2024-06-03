using System.Threading.Tasks;
using ColdMint.scripts.inventory;
using ColdMint.scripts.map;
using ColdMint.scripts.map.LayoutParsingStrategy;
using ColdMint.scripts.map.layoutStrategy;
using ColdMint.scripts.map.RoomPlacer;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.loader.sceneLoader;

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
        //Load operation prompt
        //加载操作提示
        var operationTip = GetNode<RichTextLabel>("CanvasLayer/Control/VBoxContainer/OperationTip");
        GameSceneNodeHolder.OperationTipLabel = operationTip;
        //Loaded weapon container
        //加载武器容器
        var weaponContainer = GetNode<Node2D>("WeaponContainer");
        GameSceneNodeHolder.WeaponContainer = weaponContainer;
        //Load AICharacter container
        //加载AICharacter容器
        var aiCharacterContainer = GetNode<Node2D>("AICharacterContainer");
        GameSceneNodeHolder.AICharacterContainer = aiCharacterContainer;
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
            _seedLabel.Visible = Config.IsDebug();
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
            var seedInfo = TranslationServerUtils.TranslateWithFormat("seed_info", MapGenerator.Seed);
            if (seedInfo == null)
            {
                _seedLabel.Text = $"Seed: {MapGenerator.Seed}";
            }
            else
            {
                _seedLabel.Text = seedInfo;
            }
        }

        await MapGenerator.GenerateMap();
    }
}