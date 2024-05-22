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
        MapGenerator.MapRoot = GetNode<Node>("MapRoot");
        MapGenerator.LayoutStrategy = new TestLayoutStrategy();
        MapGenerator.LayoutParsingStrategy = new SequenceLayoutParsingStrategy();
        MapGenerator.RoomPlacementStrategy = new PatchworkRoomPlacementStrategy();
        //Test the seeds used
        //2531276826 Right-Down和Left-Down匹配成功
        //4208831693 Left-Down和Right-Up匹配成功
        MapGenerator.Seed = "2531276826";
        await MapGenerator.GenerateMap();
    }
}