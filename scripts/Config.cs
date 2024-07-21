using System;
using System.IO;
using System.Text;
using Godot;
using Environment = System.Environment;

namespace ColdMint.scripts;

public static class Config
{
    /// <summary>
    /// <para>Loot table ID</para>
    /// <para>战利品表ID</para>
    /// </summary>
    public static class LootListId
    {
        /// <summary>
        /// <para>A trophy table for testing</para>
        /// <para>测试用的战利品表</para>
        /// </summary>
        public const string Test = "test";
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
    /// <para>Path of the App configuration file</para>
    /// <para>App配置文件路径</para>
    /// </summary>
    public const string AppConfigPath = "res://AppConfig.yaml";

    /// <summary>
    /// <para>The percentage of speed reduced after a thrown item hits an enemy</para>
    /// <para>抛出的物品击中敌人后减少的速度百分比</para>
    /// </summary>
    public const float ThrownItemsHitEnemiesReduceSpeedByPercentage = 0.5f;

    /// <summary>
    /// <para>How much blood does a heart represent</para>
    /// <para>一颗心代表多少血量</para>
    /// </summary>
    public const int HeartRepresentsHealthValue = 4;

    /// <summary>
    /// <para>The maximum number of stacked items in a single inventory</para>
    /// <para>单个物品栏最大堆叠的物品数量</para>
    /// </summary>
    public const int MaxStackQuantity = 99;

    /// <summary>
    /// <para>Text change buffering Time How long does it take to execute the actual event after an event with a text change listener is triggered? (Anti-shake processing time), unit: milliseconds</para>
    /// <para>当添加了文本改变监听器的事件被触发后，多长时间后执行实际事件？（防抖处理时长），单位：毫秒</para>
    /// </summary>
    public const long TextChangesBuffetingDuration = 300;

    /// <summary>
    /// <para>Operation prompts, function key text color</para>
    /// <para>操作提示内，功能键文本颜色</para>
    /// </summary>
    public const string OperationTipActionColor = "#2b8a3e";


    /// <summary>
    /// <para>Company/Creator name</para>
    /// <para>公司/创作者名字</para>
    /// </summary>
    public const string CompanyName = "ColdMint";
    
    /// <summary>
    /// <para>Solution Name</para>
    /// <para>解决方案名称</para>
    /// </summary>
    public static string SolutionName = "ColdMint.Traveler";

    /// <summary>
    /// <para>How many item slots are there on the shortcut bar</para>
    /// <para>快捷栏上有多少个物品槽</para>
    /// </summary>
    public const int HotBarSize = 9;


    /// <summary>
    /// <para>UserID</para>
    /// <para>用户ID</para>
    /// </summary>
    public const string UserId = "DefaultUser";

    /// <summary>
    /// <para>Whether version isolation is enabled</para>
    /// <para>是否启用版本隔离</para>
    /// </summary>
    public const bool EnableVersionIsolation = true;

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
    /// <returns></returns>
    public static OsEnum GetOs()
    {
        if (OS.HasFeature("editor"))
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
        if (EnableVersionIsolation)
        {
            return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CompanyName,
                ProjectSettings.GetSetting("application/config/name").AsString(), UserId,
                ProjectSettings.GetSetting("application/config/version").AsString());
        }
        else
        {
            return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CompanyName,
                ProjectSettings.GetSetting("application/config/name").AsString(), UserId,
                DefaultVersionName);
        }
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


    /// <summary>
    /// <para>Physical collision layer number</para>
    /// <para>物理碰撞层 序号</para>
    /// </summary>
    public static class LayerNumber
    {
        public const int RoomArea = 1;
        public const int Ground = 2;
        public const int Player = 3;
        public const int PickAbleItem = 4;
        public const int Projectile = 5;
        public const int Platform = 6;
        public const int Mob = 7;
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