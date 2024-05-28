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
        //加载血条场景
        var healthBarUi = GetNode<HealthBarUi>("CanvasLayer/Control/VBoxContainer/HealthBarUi");
        GameSceneNodeHolder.HealthBarUi = healthBarUi;
        //加载HotBar
        var hotBar = GetNode<HotBar>("CanvasLayer/Control/VBoxContainer/HotBar");
        GameSceneNodeHolder.HotBar = hotBar;
        //加载操作提示
        var operationTip = GetNode<RichTextLabel>("CanvasLayer/Control/VBoxContainer/OperationTip");
        GameSceneNodeHolder.OperationTipLabel = operationTip;
        //加载武器容器
        var weaponContainer = GetNode<Node2D>("WeaponContainer");
        GameSceneNodeHolder.WeaponContainer = weaponContainer;
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
        MapGenerator.MapGenerationCompleteEvent += (_) =>
        {
        };
        MapGenerator.MapGenerationStartEvent+= (_) =>
        {
           
        };
        await GenerateMap();
    }

    /// <summary>
    /// <para>Generate map</para>
    /// <para>生成地图</para>
    /// </summary>
    private async Task GenerateMap()
    {
        //2757235769 房间边框重叠！
        //4175259928 房间内容重叠！
        //212782913 起始房间重叠！
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