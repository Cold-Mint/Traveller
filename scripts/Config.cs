using System;
using System.IO;
using System.Text;
using Godot;
using Environment = System.Environment;

namespace ColdMint.scripts;

public static class Config
{
    /// <summary>
    /// <para>Difficulty</para>
    /// <para>游戏难度</para>
    /// </summary>
    public static class Difficulty
    {
        /// <summary>
        /// <para>Simple mode</para>
        /// <para>简单模式</para>
        /// </summary>
        public const int Easy = 0;

        /// <summary>
        /// <para>Normal mode</para>
        /// <para>正常模式</para>
        /// </summary>
        public const int Normal = 1;

        /// <summary>
        /// <para>Hard mode</para>
        /// <para>困难模式</para>
        /// </summary>
        public const int Hard = 2;
    }


    public static class CommandNames
    {
        /// <summary>
        /// <para>Map</para>
        /// <para>地图相关命令</para>
        /// </summary>
        /// <remarks>
        ///<para>Set the map generation parameters to regenerate the map</para>
        ///<para>设置地图生成参数，重新生成地图</para>
        /// </remarks>
        public const string Map = "map";

        /// <summary>
        /// <para>World seed related orders</para>
        /// <para>世界种子相关命令</para>
        /// </summary>
        /// <remarks>
        ///<para>View, set, or redistribute world seeds</para>
        ///<para>查看，设置，或重新分配世界种子</para>
        /// </remarks>
        public const string Seed = "seed";

        /// <summary>
        /// <para>Camera related commands</para>
        /// <para>相机相关命令</para>
        /// </summary>
        /// <remarks>
        ///<para>Open free perspective</para>
        ///<para>开启自由视角</para>
        /// </remarks>
        public const string Camera = "camera";
        public const string Debug = "debug";

        /// <summary>
        /// <para>Fog related command</para>
        /// <para>迷雾相关命令</para>
        /// </summary>
        public const string Fog = "fog";

        public const string Give = "give";

        public const string Player = "player";
        public const string AssetsRegistry = "assetsRegistry";
        public const string Room = "room";
    }

    public static class ColorConfig
    {
        /// <summary>
        /// <para>Friendly color</para>
        /// <para>友善的颜色</para>
        /// </summary>
        public static Color FriendlyColor = new("#51cf66");

        /// <summary>
        /// <para>Friendly Colors Background Color</para>
        /// <para>友善的颜色背景色</para>
        /// </summary>
        public static Color FriendlyBackgroundColor = new("#d3f9d8");

        /// <summary>
        /// <para>Enemy Color</para>
        /// <para>敌人颜色</para>
        /// </summary>
        public static Color EnemyColor = new("#ff6b6b");

        /// <summary>
        /// <para>Enemy Background Color</para>
        /// <para>敌人颜色背景色</para>
        /// </summary>
        public static Color EnemyBackgroundColor = new("#ffe3e3");

        /// <summary>
        ///<para>Border Color</para>
        ///<para>边框颜色</para>
        /// </summary>
        public static Color BorderColor = new("#f8f9fa");

        /// <summary>
        /// <para>Converts colors from the RGB (0-255) format to the format used by the shader (0.0-1.0).</para>
        /// <para>将颜色从 RGB（0-255）格式转换为着色器使用的格式（0.0-1.0）。</para>
        /// </summary>
        /// <param name="originalColor">
        ///<para>The original color with RGB components in the range 0 to 255.</para>
        /// <para>原始颜色，其 RGB 分量在 0 到 255 的范围内。</para>
        /// </param>
        /// <returns>
        /// <para>The converted color has an RGB component in the range of 0.0 to 1.0.</para>
        /// <para>转换后的颜色，其 RGB 分量在 0.0 到 1.0 的范围内。</para>
        /// </returns>
        public static Color ToShaderParameter(Color originalColor)
        {
            if (originalColor.R <= 1f || originalColor.G <= 1f || originalColor.B <= 1f)
            {
                throw new ArgumentException("The color components must be in the range of 0 to 255.");
            }

            return new Color(originalColor.R / 255f, originalColor.G / 255f, originalColor.B / 255f, originalColor.A);
        }
    }


    /// <summary>
    /// <para>Item type code</para>
    /// <para>物品类型代码</para>
    /// </summary>
    public static class ItemTypeCode
    {
        /// <summary>
        /// <para>All</para>
        /// <para>全部</para>
        /// </summary>
        /// <remarks>
        ///<para>Special item type code, do not set the item type code to All.</para>
        ///<para>特殊的物品类型代码，不要将物品的类型代码设置为All。</para>
        /// </remarks>
        public const int All = -1;

        /// <summary>
        /// <para>Unknown type</para>
        /// <para>未知类型</para>
        /// </summary>
        public const int Unknown = 0;

        /// <summary>
        /// <para>Weapon</para>
        /// <para>武器</para>
        /// </summary>
        public const int Weapon = 1;

        /// <summary>
        /// <para>Spell</para>
        /// <para>法术</para>
        /// </summary>
        public const int Spell = 2;

        /// <summary>
        /// <para>Backpack</para>
        /// <para>背包</para>
        /// </summary>
        public const int Backpack = 3;
    }

    public static class ObjectType
    {
        /// <summary>
        /// <para>Query all types</para>
        /// <para>查询全部类型</para>
        /// </summary>
        public const int All = 0;


        /// <summary>
        /// <para>Player</para>
        /// <para>玩家</para>
        /// </summary>
        public const int Player = 1;

        /// <summary>
        /// <para>Characters, including players</para>
        /// <para>角色，包含玩家</para>
        /// </summary>
        public const int Character = 2;

        /// <summary>
        /// <para>Item</para>
        /// <para>物品</para>
        /// </summary>
        public const int Item = 3;
    }

    public static class DynamicSuggestionID
    {
        public const string Boolean = "Boolean";
        public const string Item = "Item";
        public const string ObjectSelector = "ObjectSelector";
        public const string Room = "Room";
    }

    public static class MapGeneratorStopCode
    {
        /// <summary>
        /// <para>Normal end</para>
        /// <para>正常结束</para>
        /// </summary>
        public const int Normal = 0;

        /// <summary>
        /// <para>Map Generator is prohibited from creating new generation tasks because the parameters are incomplete or working.</para>
        /// <para>地图生成器因为参数不全或者正在工作中，被禁止了新的生成任务。</para>
        /// </summary>
        public const int BePrevented = 1;

        /// <summary>
        /// <para>An error occurred while initializing the map generator.</para>
        /// <para>初始化地图生成器时出现了错误。</para>
        /// </summary>
        public const int InitializationFailure = 2;

        /// <summary>
        /// <para>Parameter incompleteness detected at run time</para>
        /// <para>运行时检测到参数不全</para>
        /// </summary>
        public const int ParameterIncompletenessDetected = 3;

        /// <summary>
        /// <para>The level map is not available</para>
        /// <para>关卡图不可用</para>
        /// </summary>
        public const int LevelGraphIsNotAvailable = 4;

        /// <summary>
        /// <para>Initial room placement failed</para>
        /// <para>初始房间放置失败</para>
        /// </summary>
        public const int InitialRoomPlacementFailed = 5;
    }

    public static class EntityCollisionMode
    {
        /// <summary>
        /// <para>There will be no collisions with players or creatures</para>
        /// <para>不会与玩家或生物发生碰撞</para>
        /// </summary>
        public const int None = 0;

        /// <summary>
        /// <para>Can only collide with players</para>
        /// <para>仅和玩家能够产生碰撞</para>
        /// </summary>
        public const int OnlyPlayers = 1;

        /// <summary>
        /// <para>Ability to collide with players and creatures</para>
        /// <para>能够和玩家还有生物产生碰撞</para>
        /// </summary>
        public const int PlayersAndEntity = 2;
    }


    /// <summary>
    /// <para>Camp ID</para>
    /// <para>阵营ID</para>
    /// </summary>
    public static class CampId
    {
        /// <summary>
        /// <para>Default camp</para>
        /// <para>表示默认阵营</para>
        /// </summary>
        public const string Default = "Default";

        /// <summary>
        /// <para>Demon camp</para>
        /// <para>魔族阵营</para>
        /// </summary>
        public const string Mazoku = "Mazoku";

        /// <summary>
        /// <para>Aborigines</para>
        /// <para>原住民</para>
        /// </summary>
        public const string Aborigines = "Aborigines";
    }

    /// <summary>
    /// <para>Minimum size of item slot on Android platform</para>
    /// <para>物品槽在安卓平台上的最小尺寸</para>
    /// </summary>
    public static readonly Vector2 ItemSlotNodeMinimumSizeInAndroid = new(48f, 48f);

    /// <summary>
    /// <para>Scale of the room preview view</para>
    /// <para>房间预览图的缩放</para>
    /// </summary>
    public const float RoomPreviewScale = 3f;

    /// <summary>
    /// <para>Barrage velocity</para>
    /// <para>弹幕速度</para>
    /// </summary>
    public const float BarrageSpeed = 50;

    /// <summary>
    /// <para>How much blood does a heart represent</para>
    /// <para>一颗心代表多少血量</para>
    /// </summary>
    public const int HeartRepresentsHealthValue = 4;

    /// <summary>
    /// <para>The name of the mod manifest file</para>
    /// <para>模组清单文件的名字</para>
    /// </summary>
    public const string ModManifestFileName = "ModManifest.yaml";

    /// <summary>
    /// <para>Text change buffering Time How long does it take to execute the actual event after an event with a text change listener is triggered? (Anti-shake processing time), unit: milliseconds</para>
    /// <para>当添加了文本改变监听器的事件被触发后，多长时间后执行实际事件？（防抖处理时长），单位：毫秒</para>
    /// </summary>
    public const long TextChangesBuffetingDuration = 300;

    /// <summary>
    /// <para>Company/Creator name</para>
    /// <para>公司/创作者名字</para>
    /// </summary>
    public const string CompanyName = "ColdMint";

    /// <summary>
    /// <para>Module life handler name</para>
    /// <para>模组生命周期处理器名称</para>
    /// </summary>
    public const string ModLifecycleHandlerName = "ModLifecycleHandler";

    /// <summary>
    /// <para>Solution Name</para>
    /// <para>解决方案名称</para>
    /// </summary>
    public const string SolutionName = "ColdMint.Traveler";

    /// <summary>
    /// <para>How many item slots are there on the shortcut bar</para>
    /// <para>快捷栏上有多少个物品槽</para>
    /// </summary>
    public const int HotBarSize = 9;


    /// <summary>
    /// <para>Whether version isolation is enabled</para>
    /// <para>是否启用版本隔离</para>
    /// </summary>
    public static bool EnableVersionIsolation()
    {
        //By default, we enable version isolation, but special feature identifiers can be set to disable version isolation.
        //默认情况，我们启用版本隔离，但是可以设置特殊的功能标识来禁用版本隔离。
        return !OS.HasFeature("disableVersionIsolation");
    }

    /// <summary>
    /// <para>Whether to enable Mod</para>
    /// <para>是否启用Mod</para>
    /// </summary>
    /// <returns></returns>
    public static bool EnableMod()
    {
        return OS.HasFeature("enableMod");
    }

    /// <summary>
    /// <para>Default version name</para>
    /// <para>默认的版本名称</para>
    /// </summary>
    /// <remarks>
    ///<para>Used when version isolation is disabled</para>
    ///<para>在禁用版本隔离时用的</para>
    /// </remarks>
    public const string DefaultVersionName = "Default";

    /// <summary>
    /// <para>EmptyVariant</para>
    /// <para>空变量</para>
    /// </summary>
    public static readonly Variant EmptyVariant = new();

    /// <summary>
    /// <para>Blank string</para>
    /// <para>空白字符串</para>
    /// </summary>
    public const string? EmptyString = null;


    /// <summary>
    /// <para>IsDebug</para>
    /// <para>是否为Debug模式</para>
    /// </summary>
    /// <returns></returns>
    public static bool IsDebug()
    {
        return OS.HasFeature("debug");
    }

    /// <summary>
    /// <para>Whether to run on the editor</para>
    /// <para>是否在编辑器上运行</para>
    /// </summary>
    /// <returns></returns>
    public static bool IsEditor()
    {
        return OS.HasFeature("editor");
    }

    /// <summary>
    /// <para>ItemType</para>
    /// <para>物品类型</para>
    /// </summary>
    public static class ItemType
    {
        /// <summary>
        /// <para>Unknown</para>
        /// <para>未知的</para>
        /// </summary>
        public const int Unknown = 0;

        /// <summary>
        /// <para>Placeholder</para>
        /// <para>占位符</para>
        /// </summary>
        public const int Placeholder = 1;

        /// <summary>
        /// <para>Packsack</para>
        /// <para>背包</para>
        /// </summary>
        public const int Packsack = 2;

        /// <summary>
        /// <para>ProjectileWeapon</para>
        /// <para>远程武器</para>
        /// </summary>
        public const int ProjectileWeapon = 3;

        /// <summary>
        /// <para>Spell</para>
        /// <para>法术</para>
        /// </summary>
        /// <remarks>
        ///<para>Type of special item used in Projectile weapons</para>
        ///<para>用于远程武器内的特殊物品类型</para>
        /// </remarks>
        public const int Spell = 4;

        /// <summary>
        /// <para>Common item types</para>
        /// <para>普通的物品类型</para>
        /// </summary>
        public const int Item = 5;
    }

    /// <summary>
    /// <para>Room Injector ID</para>
    /// <para>房间注入器ID</para>
    /// </summary>
    public static class RoomInjectionProcessorId
    {
        /// <summary>
        /// <para>Chance</para>
        /// <para>概率的</para>
        /// </summary>
        public const string Chance = "Chance";

        /// <summary>
        /// <para>TimeInterval</para>
        /// <para>时间范围的</para>
        /// </summary>
        public const string TimeInterval = "TimeInterval";
    }

    public class ZIndexManager
    {
        /// <summary>
        /// <para>Floating icon</para>
        /// <para>悬浮图标</para>
        /// </summary>
        public const int FloatingIcon = 1;
    }

    /// <summary>
    /// <para>Item data changes the event type</para>
    /// <para>物品数据改变事件类型</para>
    /// </summary>
    public enum ItemDataChangeEventType
    {
        /// <summary>
        /// <para>add</para>
        /// <para>添加</para>
        /// </summary>
        Add,

        /// <summary>
        /// <para>Quantity Added</para>
        /// <para>物品数量增加</para>
        /// </summary>
        QuantityAdded,

        /// <summary>
        /// <para>remove</para>
        /// <para>移除</para>
        /// </summary>
        Remove,

        /// <summary>
        /// <para>Replace</para>
        /// <para>被替换</para>
        /// </summary>
        Replace,

        /// <summary>
        /// <para>Clear</para>
        /// <para>被清空</para>
        /// </summary>
        Clear
    }

    public enum OsEnum
    {
        //unknown
        //未知
        Unknown,

        //Runs on Android (non-web browser)
        //在 Android 上运行（非 Web 浏览器）
        Android,

        //Runs on Linux (non-web browser)
        //在 Linux 上运行（非 Web 浏览器）
        Linux,

        //Runs on macOS (non-Web browser)
        //在 macOS 上运行（非 Web 浏览器）
        Macos,

        //Runs on iOS (non-Web browser)
        //在 iOS 上运行（非 Web 浏览器）
        Ios,

        //Runs on Windows
        //在 Windows 上运行
        Windows,

        //The host operating system is a web browser
        //宿主操作系统是网页浏览器
        Web,

        //Running on editor
        //在编辑器内运行
        Editor
    }

    /// <summary>
    /// <para>Get what platform is currently running on</para>
    /// <para>获取当前在什么平台上运行</para>
    /// </summary>
    /// <param name="containEditor">
    ///<para>Whether to include an editor environment</para>
    ///<para>是否包含编辑器环境</para>
    /// </param>
    /// <returns></returns>
    public static OsEnum GetOs(bool containEditor = false)
    {
        if (containEditor && OS.HasFeature("editor"))
        {
            return OsEnum.Editor;
        }

        if (OS.HasFeature("windows"))
        {
            return OsEnum.Windows;
        }

        if (OS.HasFeature("android"))
        {
            return OsEnum.Android;
        }

        if (OS.HasFeature("linux"))
        {
            return OsEnum.Linux;
        }

        if (OS.HasFeature("web"))
        {
            return OsEnum.Web;
        }

        if (OS.HasFeature("macos"))
        {
            return OsEnum.Macos;
        }

        if (OS.HasFeature("ios"))
        {
            return OsEnum.Ios;
        }

        return OsEnum.Unknown;
    }

    /// <summary>
    /// <para>Get the game version</para>
    /// <para>获取游戏版本</para>
    /// </summary>
    /// <returns></returns>
    public static string GetVersion()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(ProjectSettings.GetSetting("application/config/version").AsString());
        stringBuilder.Append(IsDebug() ? "_debug" : "_release");
        return stringBuilder.ToString();
    }

    /// <summary>
    /// <para>GetGameDataDirectory</para>
    /// <para>获取游戏数据目录</para>
    /// </summary>
    /// <returns></returns>
    public static string GetGameDataDirectory()
    {
        if (GetOs() == OsEnum.Android)
        {
            if (EnableVersionIsolation())
            {
                return Path.Join(ProjectSettings.GlobalizePath("user://"),
                    ProjectSettings.GetSetting("application/config/version").AsString());
            }

            return Path.Join(ProjectSettings.GlobalizePath("user://"), DefaultVersionName);
        }

        //For desktop platforms.
        //对于桌面平台。
        if (EnableVersionIsolation())
        {
            return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CompanyName,
                ProjectSettings.GetSetting("application/config/name").AsString(),
                ProjectSettings.GetSetting("application/config/version").AsString());
        }

        return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CompanyName,
            ProjectSettings.GetSetting("application/config/name").AsString(),
            DefaultVersionName);
    }

    /// <summary>
    /// <para>GetDataBaseDirectory</para>
    /// <para>获取数据库文件夹</para>
    /// </summary>
    /// <returns></returns>
    public static string GetDataBaseDirectory()
    {
        return Path.Join(GetGameDataDirectory(), "Databases");
    }

    /// <summary>
    /// <para>GetModDataDirectory</para>
    /// <para>获取模组文件夹</para>
    /// </summary>
    /// <returns></returns>
    public static string GetModDataDirectory()
    {
        return Path.Join(GetGameDataDirectory(), "Mods");
    }

    /// <summary>
    /// <para>Get the export directory for the level graph</para>
    /// <para>获取关卡图的导出目录</para>
    /// </summary>
    /// <returns></returns>
    public static string GetLevelGraphExportDirectory()
    {
        return Path.Join(GetGameDataDirectory(), "LevelGraphs");
    }

    /// <summary>
    /// <para>The initial year of creating this game</para>
    /// <para>创建此游戏的初始年份</para>
    /// </summary>
    public const int CreationYear = 2024;

    /// <summary>
    /// <para>Tile map, dimensions of individual tiles</para>
    /// <para>瓦片地图，单个瓦片的尺寸</para>
    /// </summary>
    public const int CellSize = 32;

    /// <summary>
    /// <para>The maximum health of the default creature</para>
    /// <para>默认生物的最大血量</para>
    /// </summary>
    public const int DefaultMaxHp = 100;

    /// <summary>
    /// <para>The default durability of furniture</para>
    /// <para>家具的默认耐久度</para>
    /// </summary>
    public const int DefaultMaxDurability = 50;

    /// <summary>
    /// <para>When a creature takes damage, how long to hide the bloodline again</para>
    /// <para>生物受到伤害时，要在多长时间后再次隐藏血条</para>
    /// </summary>
    public static TimeSpan HealthBarDisplaysTime = TimeSpan.FromSeconds(2);

    /// <summary>
    /// <para>Text size of critical hit damage</para>
    /// <para>暴击伤害的文本大小</para>
    /// </summary>
    public const int CritDamageTextSize = 33;

    /// <summary>
    /// <para>The default map generation overlap detection delay</para>
    /// <para>默认的地图生成重叠检测延迟</para>
    /// </summary>
    /// <remarks>
    ///<para>Higher latencies are more stable and less likely to overlap.</para>
    ///<para>越高的延迟越稳不易重叠。</para>
    /// </remarks>
    public const int DefaultOverlapDetectionDelay = 30;

    /// <summary>
    /// <para>Crit damage multiplier</para>
    /// <para>暴击伤害乘数</para>
    /// </summary>
    /// <remarks>
    ///<para>How much damage to increase after a critical strike</para>
    ///<para>造成暴击后要将伤害提升到原有的多少倍</para>
    /// </remarks>
    public const float CriticalHitMultiplier = 2f;

    /// <summary>
    /// <para>Text size of normal damage</para>
    /// <para>普通伤害的文本大小</para>
    /// </summary>
    public const int NormalDamageTextSize = 22;

    /// <summary>
    /// <para>Horizontal speed of damage numbers</para>
    /// <para>伤害数字的水平速度</para>
    /// </summary>
    public const int HorizontalSpeedOfDamageNumbers = 3;


    /// <summary>
    /// <para>VerticalVelocityOfDamageNumbers</para>
    /// <para>伤害数字的垂直速度</para>
    /// </summary>
    public const int VerticalVelocityOfDamageNumbers = 5;


    public static class OffsetAngleMode
    {
        /// <summary>
        /// <para>Random(Default)</para>
        /// <para>随机的（默认）</para>
        /// </summary>
        public const int Random = 0;

        /// <summary>
        /// <para>AlwaysSame</para>
        /// <para>永远不变的偏移角度</para>
        /// </summary>
        public const int AlwaysSame = 1;

        /// <summary>
        /// <para>Cross</para>
        /// <para>交叉变换</para>
        /// </summary>
        public const int Cross = 2;
    }

    /// <summary>
    /// <para>Physical collision layer number</para>
    /// <para>物理碰撞层 序号</para>
    /// </summary>
    public static class LayerNumber
    {
        public const int RoomArea = 1;

        /// <summary>
        /// <para>Floor</para>
        /// <para>地板</para>
        /// </summary>
        public const int Floor = 2;

        public const int Player = 3;
        public const int PickAbleItem = 4;
        public const int Projectile = 5;

        /// <summary>
        /// <para>Platform</para>
        /// <para>平台</para>
        /// </summary>
        public const int Platform = 6;

        public const int Mob = 7;

        /// <summary>
        /// <para>Wall</para>
        /// <para>墙壁</para>
        /// </summary>
        public const int Wall = 8;

        /// <summary>
        /// <para>Furniture</para>
        /// <para>家具</para>
        /// </summary>
        public const int Furniture = 9;

        /// <summary>
        /// <para>WeaponDamageArea</para>
        /// <para>武器伤害区域</para>
        /// </summary>
        public const int WeaponDamageArea = 10;

        public const int Barrier = 11;

        /// <summary>
        /// <para>NonPickupItem</para>
        /// <para>不能被捡起的物品</para>
        /// </summary>
        /// <remarks>
        ///<para>For example: A heart that restores health after being touched.</para>
        ///<para>例如：触碰后会恢复健康值的红心。</para>
        /// </remarks>
        public const int NonPickupItem = 12;

        /// <summary>
        /// <para>ProjectileBarrier</para>
        /// <para>抛射体障碍</para>
        /// </summary>
        public const int ProjectileBarrier = 13;
    }

    /// <summary>
    /// <para>TileMapLayerName</para>
    /// <para>瓦片节点名称</para>
    /// </summary>
    public static class TileMapLayerName
    {
        /// <summary>
        /// <para>Ground Layer</para>
        /// <para>地面层</para>
        /// </summary>
        /// <remarks>
        ///<para>There are collision nodes on which players and creatures can stand.</para>
        ///<para>拥有碰撞节点，玩家和生物可以站在上面。</para>
        /// </remarks>
        public const string Ground = "Ground";

        /// <summary>
        /// <para>Barrier</para>
        /// <para>屏障</para>
        /// </summary>
        public const string Barrier = "Barrier";

        /// <summary>
        /// <para>Background decorative layer</para>
        /// <para>背景装饰层</para>
        /// </summary>
        public const string BackgroundDecoration = "BackgroundDecoration";

        /// <summary>
        /// <para>Background wall layer</para>
        /// <para>背景墙</para>
        /// </summary>
        public const string BackgroundWall = "BackgroundWall";
    }

    public static class RoomDataTag
    {
        /// <summary>
        /// <para>Mark the starting room</para>
        /// <para>起点房间的标记</para>
        /// </summary>
        public const string StartingRoom = "StartingRoom";
    }

    /// <summary>
    /// <para>Specify the type of damage used in the game</para>
    /// <para>指定游戏内使用的伤害类型</para>
    /// </summary>
    public static class DamageType
    {
        /// <summary>
        /// <para>physical injury</para>
        /// <para>物理伤害</para>
        /// </summary>
        public const int Physical = 1;

        /// <summary>
        /// <para>Magic damage</para>
        /// <para>魔法伤害</para>
        /// </summary>
        public const int Magic = 2;
    }
}