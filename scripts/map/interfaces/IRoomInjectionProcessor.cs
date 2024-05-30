using System.Threading.Tasks;
using Godot;

namespace ColdMint.scripts.map.interfaces;

/// <summary>
/// <para>Room injection processor</para>
/// <para>房间注入处理器</para>
/// </summary>
public interface IRoomInjectionProcessor
{
    /// <summary>
    /// <para>The room injection processor has an ID</para>
    /// <para>房间注入处理器有一个ID</para>
    /// </summary>
    /// <returns></returns>
    public string GetId();

    /// <summary>
    /// <para>Whether it can be placed on the map</para>
    /// <para>是否能够被放置到地图内</para>
    /// </summary>
    /// <param name="randomNumberGenerator">
    ///<para>Random probability generator based on world seed</para>
    ///<para>根据世界种子确定的随机概率生成器</para>
    /// </param>
    /// <param name="jsonConfigData">
    ///<para>Inject data into the processor</para>
    ///<para>注入处理器的数据</para>
    /// </param>
    /// <returns></returns>
    public Task<bool> CanBePlaced(RandomNumberGenerator randomNumberGenerator, string? jsonConfigData);
}