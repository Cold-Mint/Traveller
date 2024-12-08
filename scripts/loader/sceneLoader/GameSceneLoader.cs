using System.Threading.Tasks;
using ColdMint.scripts.debug;
using ColdMint.scripts.loader.uiLoader;
using ColdMint.scripts.map;
using ColdMint.scripts.map.events;
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

    public override Task InitializeData()
    {
        RenderingServer.SetDefaultClearColor(Color.FromHsv(0, 0, 0));
        //Load DynamicUiGroup
        //加载动态Ui容器
        var dynamicUiGroup = GetNode<UiGroup>("CanvasLayer/DynamicUiGroup");
        GameSceneDepend.DynamicUiGroup = dynamicUiGroup;
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
        //load the dynamicDamageAreaContainer
        //加载动态伤害区域容器
        var dynamicDamageAreaContainer = GetNode<Node2D>("DynamicDamageAreaContainer");
        GameSceneDepend.DynamicDamageAreaContainer = dynamicDamageAreaContainer;
        //Load the pickable container
        //加载可拾捡物容器
        var pickAbleContainer = GetNode<Node2D>("PickAbleContainer");
        GameSceneDepend.PickAbleContainer = pickAbleContainer;
        InstantiateGui(Config.GetOs() == Config.OsEnum.Android ? "res://prefab/ui/gameGuiMobile.tscn" : "res://prefab/ui/gameGuiDesktop.tscn");
        return Task.CompletedTask;
    }


    /// <summary>
    /// <para>Instantiate the game's GUI</para>
    /// <para>实例化游戏的GUI</para>
    /// </summary>
    /// <param name="path"></param>
    private void InstantiateGui(string path)
    {
        var packedScene = ResourceLoader.Load<PackedScene>(path);
        var gameGuiTemplate = NodeUtils.InstantiatePackedScene<GameGuiTemplate>(packedScene);
        if (gameGuiTemplate == null)
        {
            LogCat.LogError("game_gui_template_is_null");
            return;
        }
        NodeUtils.CallDeferredAddChild(GetNode<Control>("CanvasLayer/GameGui"), gameGuiTemplate);
        gameGuiTemplate.Ready += () =>
        {
            var debugMode = Config.IsDebug();
            if (gameGuiTemplate.RecreateMapButton != null)
            {
                gameGuiTemplate.RecreateMapButton.Visible = debugMode;
                gameGuiTemplate.RecreateMapButton.Pressed += () => { _ = GenerateMap(); };
            }

            if (gameGuiTemplate.SeedLabel != null)
            {
                gameGuiTemplate.SeedLabel.Visible = debugMode;
            }
        };
        GameSceneDepend.GameGuiTemplate = gameGuiTemplate;
    }

    public override async Task LoadScene()
    {
        MapGenerator.MapRoot = GetNode<Node>("MapRoot");
        MapGenerator.LayoutStrategy = new TestLayoutStrategy();
        MapGenerator.LayoutParsingStrategy = new SequenceLayoutParsingStrategy();
        MapGenerator.RoomPlacementStrategy = new PatchworkRoomPlacementStrategy();
        EventBus.GameOverEvent += OnGameOverEvent;
        await GenerateMap();
    }

    private async Task OnGameOverEvent(GameOverEvent gameOverEvent)
    {
        if (GameSceneDepend.WeaponContainer != null)
        {
            NodeUtils.DeleteAllChild(GameSceneDepend.WeaponContainer);
        }
        if (GameSceneDepend.PacksackContainer != null)
        {
            NodeUtils.DeleteAllChild(GameSceneDepend.PacksackContainer);
        }
        if (GameSceneDepend.SpellContainer != null)
        {
            NodeUtils.DeleteAllChild(GameSceneDepend.SpellContainer);
        }
        await GenerateMap();
        var replayEvent = new GameReplayEvent();
        EventBus.GameReplayEvent?.Invoke(replayEvent);
    }

    public override void _ExitTree()
    {
        EventBus.GameOverEvent -= OnGameOverEvent;
    }

    /// <summary>
    /// <para>Generate map</para>
    /// <para>生成地图</para>
    /// </summary>
    private async Task GenerateMap()
    {
        MapGenerator.Seed = GuidUtils.GetGuid();
        if (GameSceneDepend.GameGuiTemplate?.SeedLabel != null)
        {
            //If you have a seedLabel, then set the seed to it.
            //如果有seedLabel，那么将种子设置上去。
            var seedInfo = TranslationServerUtils.TranslateWithFormat("ui_seed_info", MapGenerator.Seed);
            GameSceneDepend.GameGuiTemplate.SeedLabel.Text = seedInfo ?? $"Seed: {MapGenerator.Seed}";
        }
        await MapGenerator.GenerateMap();
    }
}