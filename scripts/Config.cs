using System;
using System.IO;
using System.Text;
using Godot;
using Environment = System.Environment;

namespace ColdMint.scripts;

public static class Config
{
    public class BehaviorTreeId
    {
        /// <summary>
        /// <para>巡逻</para>
        /// <para>Patrol</para>
        /// </summary>
        public const string Patrol = "Patrol";
    }

    /// <summary>
    /// <para>BehaviorTreeResult</para>
    /// <para>行为树的结果</para>
    /// </summary>
    public class BehaviorTreeResult
    {
        /// <summary>
        /// <para>Running</para>
        /// <para>运行中</para>
        /// </summary>
        public const int Running = 0;

        /// <summary>
        /// <para>Success</para>
        /// <para>成功</para>
        /// </summary>
        public const int Success = 1;

        /// <summary>
        /// <para>Failure</para>
        /// <para>失败</para>
        /// </summary>
        public const int Failure = 2;
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
    /// <para>Company/Creator name</para>
    /// <para>公司/创作者名字</para>
    /// </summary>
    public const string CompanyName = "ColdMint";

    /// <summary>
    /// <para>How many item slots are there on the shortcut bar</para>
    /// <para>快捷栏上有多少个物品槽</para>
    /// </summary>
    public const int HotBarSize = 10;


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
    /// <para>The file name of the packet's manifest</para>
    /// <para>数据包的清单文件名</para>
    /// </summary>
    public const string DataPackManifestName = "DataPackManifest.json";

    /// <summary>
    /// <para>VerticalVelocityOfDamageNumbers</para>
    /// <para>伤害数字的垂直速度</para>
    /// </summary>
    public const int VerticalVelocityOfDamageNumbers = 5;


    /// <summary>
    /// <para>Physical collision layer number</para>
    /// <para>物理碰撞层 序号</para>
    /// </summary>
    public class LayerNumber
    {
        public const int RoomArea = 1;
        public const int Ground = 2;
        public const int Player = 3;
        public const int Weapon = 4;
        public const int Projectile = 5;
        public const int Platform = 6;
        public const int Mob = 7;
    }

    /// <summary>
    /// <para>Specify the type of damage used in the game</para>
    /// <para>指定游戏内使用的伤害类型</para>
    /// </summary>
    public class DamageType
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