using System.Threading.Tasks;
using Godot;

namespace ColdMint.scripts.map.roomInjectionProcessor;

/// <summary>
/// <para>Probabilistic room injection processor</para>
/// <para>概率的房间注入处理器</para>
/// </summary>
/// <remarks>
///<para>This processor allows you to specify a probability and then decide whether to generate a room based on that probability.</para>
///<para>此处理器允许指定一个概率，然后根据概率来决定是否生成房间。</para>
/// </remarks>
public class ChanceRoomInjectionProcessor : RoomInjectionProcessorTemplate<ChanceRoomInjectionProcessor.ConfigData>
{
    public override string GetId()
    {
        return Config.RoomInjectionProcessorId.Chance;
    }

    protected override Task<bool> OnCreateConfigData(RandomNumberGenerator randomNumberGenerator, ConfigData configData)
    {
        if (configData.Chance == null)
        {
            return Task.FromResult(false);
        }

        //Generate a random number between 1 and 10000.
        //生成1-10000的随机数。
        var round = randomNumberGenerator.Randi() % 10000 + 1;
        //If the random number is less than or equal to the probability, the room is generated.
        //如果随机数小于等于概率，则生成房间。
        return Task.FromResult(round <= configData.Chance * 100);
    }


    /// <summary>
    /// <para>Configuration Data</para>
    /// <para>配置数据</para>
    /// </summary>
    public class ConfigData
    {
        /// <summary>
        /// <para>The probability of generating this room</para>
        /// <para>生成此房间的概率</para>
        /// </summary>
        /// <para>
        ///<para>The value ranges from 1 to 100. For example, if it is set to 1.5, it means that there is a 1.5% probability of generating this room.</para>
        ///<para>支持小数，范围为1-100。例如，如果设置为1.5，则表示1.5%的概率生成此房间。</para>
        /// </para>
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public float? Chance { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }
}