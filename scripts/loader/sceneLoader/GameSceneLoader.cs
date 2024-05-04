using ColdMint.scripts.debug;
using ColdMint.scripts.inventory;
using ColdMint.scripts.loader.uiLoader;
using ColdMint.scripts.map;
using ColdMint.scripts.map.interfaces;
using ColdMint.scripts.map.room;
using ColdMint.scripts.map.roomHolder;
using ColdMint.scripts.map.RoomPlacer;
using ColdMint.scripts.map.RoomProvider;
using ColdMint.scripts.map.slotsMatcher;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.loader.sceneLoader;

public partial class GameSceneLoader : SceneLoaderTemplate
{
	private IMapGenerator _mapGenerator;
	private IMapGeneratorConfig _mapGeneratorConfig;

	public override void InitializeData()
	{
		//加载血条场景
		var healthBarUI = GetNode<HealthBarUi>("CanvasLayer/Control/VBoxContainer/HealthBarUi");
		NodeUtils.DeleteAllChild(healthBarUI);
		GameSceneNodeHolder.HealthBarUi = healthBarUI;
		//加载HotBar
		var hotBar = GetNode<HotBar>("CanvasLayer/Control/VBoxContainer/HotBar");
		NodeUtils.DeleteAllChild(hotBar);
		GameSceneNodeHolder.HotBar = hotBar;
		//加载操作提示
		var operationTip = GetNode<Label>("CanvasLayer/Control/VBoxContainer/OperationTip");
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
	}

	public override void LoadScene()
	{
		_mapGenerator.Generate(_mapGeneratorConfig);
		var packedScene = GD.Load<PackedScene>("res://prefab/entitys/Character.tscn");
		//Register players in the holder
		//在持有者内注册玩家
		var node2D = (Node2D)packedScene.Instantiate();
		GameSceneNodeHolder.Player = node2D as Player;
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
			GameSceneNodeHolder.WeaponContainer.AddChild(wn);
		}
	}
}
