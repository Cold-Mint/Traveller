using System.Threading.Tasks;

namespace ColdMint.scripts.map.interfaces;

public interface IMapGenerator
{
    /// <summary>
    /// <para>Setting the timeout period</para>
    /// <para>设置超时时间</para>
    /// </summary>
    /// <remarks>
    ///<para>Causes the engine to terminate generation after a certain amount of time.(Unit: second)</para>
    ///<para>使引擎超过一定时间后终止生成。（单位：秒）</para>
    /// </remarks>
    public int TimeOutPeriod { get; set; }

    public IRoomSlotsMatcher RoomSlotsMatcher { get; set; }
    public IRoomHolder RoomHolder { get; set; }
    public IRoomPlacer RoomPlacer { get; set; }

    public IRoomProvider RoomProvider { get; set; }

    Task Generate(IMapGeneratorConfig mapGeneratorConfig);
}