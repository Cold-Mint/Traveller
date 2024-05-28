using System.Threading.Tasks;
using ColdMint.scripts.map.dateBean;

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
    /// <para>The processing method should return to the room to place the data</para>
    /// <para>处理方法，应当返回房间放置数据</para>
    /// </summary>
    /// <returns></returns>
    public Task<RoomPlacementData?> Processor();
}