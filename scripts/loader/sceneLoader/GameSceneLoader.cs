using System.Threading.Tasks;
using ColdMint.scripts.character;
using ColdMint.scripts.debug;
using ColdMint.scripts.inventory;
using ColdMint.scripts.map;
using ColdMint.scripts.map.interfaces;
using ColdMint.scripts.map.room;
using ColdMint.scripts.map.roomHolder;
using ColdMint.scripts.map.RoomPlacer;
using ColdMint.scripts.map.RoomProvider;
using ColdMint.scripts.map.slotsMatcher;
using Godot;

namespace ColdMint.scripts.loader.sceneLoader;

public partial class GameSceneLoader : SceneLoaderTemplate
{
    private IMapGenerator? _mapGenerator;
    private IMapGeneratorConfig? _mapGeneratorConfig;

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

        _mapGenerator = new MapGenerator();
        _mapGenerator.TimeOutPeriod = 15;
        _mapGenerator.RoomHolder = new RoomHolder();
        _mapGenerator.RoomSlotsMatcher = new RoomSlotsMatcher();
        var roomProvider = new RoomProvider();
        //添加房间模板
        var initialRoom = new RoomTemplate("res://prefab/roomTemplates/dungeon/initialRoom.tscn");
        var utilityRoom = new RoomTemplate("res://prefab/roomTemplates/dungeon/utilityRoom.tscn");
        initialRoom.MaxNumber = 1;
        var horizontalCorridorWithSewer =
            new RoomTemplate("res://prefab/roomTemplates/dungeon/horizontalCorridorWithSewer.tscn");
        var horizontalCorridor = new RoomTemplate("res://prefab/roomTemplates/dungeon/horizontalCorridor.tscn");
        roomProvider.AddRoom(initialRoom);
        roomProvider.AddRoom(horizontalCorridor);
        roomProvider.AddRoom(horizontalCorridorWithSewer);
        roomProvider.AddRoom(utilityRoom);
        _mapGenerator.RoomProvider = roomProvider;

        var roomPlacer = new RoomPlacer();
        _mapGeneratorConfig = new MapGeneratorConfig(GetNode<Node2D>("MapRoot"), 1);
        roomPlacer.MapGeneratorConfig = _mapGeneratorConfig;
        _mapGenerator.RoomPlacer = roomPlacer;
        return Task.CompletedTask;
    }

    public override async Task LoadScene()
    {
        if (_mapGenerator == null)
        {
            LogCat.LogError("map_generator_is_not_set_up");
            return;
        }

        if (_mapGeneratorConfig == null)
        {
            LogCat.LogError("map_generator_is_not_configured");
            return;
        }

        await _mapGenerator.Generate(_mapGeneratorConfig);
        var packedScene = GD.Load<PackedScene>("res://prefab/entitys/Character.tscn");
        //Register players in the holder
        //在持有者内注册玩家
        var node2D = (Node2D)packedScene.Instantiate();
        if (node2D is Player player)
        {
            GameSceneNodeHolder.Player = player;
            //Allow the player to pick up items.
            //使玩家可以捡起物品。
            player.ItemContainer = GameSceneNodeHolder.HotBar;
        }

        var gameRoot = GetNode<Node2D>(".");
        gameRoot.AddChild(node2D);
        node2D.Position = new Vector2(55, 70);

        var delivererOfDarkMagicPackedScene = GD.Load<PackedScene>("res://prefab/entitys/DelivererOfDarkMagic.tscn");
        var delivererOfDarkMagicPackedSceneNode2D = (Node2D)delivererOfDarkMagicPackedScene.Instantiate();
        gameRoot.AddChild(delivererOfDarkMagicPackedSceneNode2D);
        delivererOfDarkMagicPackedSceneNode2D.Position = new Vector2(70, 70);

        //Load a weapon
        //加载武器
        var w = GD.Load<PackedScene>("res://prefab/weapons/staffOfTheUndead.tscn");
        for (int i = 0; i < 3; i++)
        {
            var wn = (Node2D)w.Instantiate();
            wn.Position = new Vector2(55, 90);
            var weaponContainer = GameSceneNodeHolder.WeaponContainer;
            weaponContainer?.AddChild(wn);
        }
    }
}