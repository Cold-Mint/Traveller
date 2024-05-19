using System.Threading.Tasks;
using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using ColdMint.scripts.inventory;
using ColdMint.scripts.map;
using ColdMint.scripts.map.LayoutParsingStrategy;
using ColdMint.scripts.map.layoutStrategy;
using ColdMint.scripts.map.room;
using ColdMint.scripts.map.roomHolder;
using ColdMint.scripts.map.RoomPlacer;
using ColdMint.scripts.map.RoomProvider;
using ColdMint.scripts.map.slotsMatcher;
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
        MapGenerator.LayoutStrategy = new TestLayoutStrategy();
        MapGenerator.LayoutParsingStrategy = new SequenceLayoutParsingStrategy();
    }
}