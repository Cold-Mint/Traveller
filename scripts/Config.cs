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
    public static readonly string? EmptyString = null;


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
                return Path.Join(ProjectSettings.GlobalizePath("user://"), ProjectSettings.GetSetting("application/config/version").AsString());

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