namespace ColdMint.scripts.map.interfaces;

/// <summary>
/// <para>Room provider</para>
/// <para>房间提供者</para>
/// </summary>
/// <remarks>
///<para>Responsible for providing room templates for map generator.</para>
///<para>负责为地图生成器提供房间模板。</para>
/// </remarks>
public interface IRoomProvider
{
    /// <summary>
    /// <para>Initial room</para>
    /// <para>初始房间</para>
    /// </summary>
    IRoomTemplate? InitialRoom { get; set; }


    /// <summary>
    /// <para>Acquire room assets</para>
    /// <para>获取房间资产</para>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    IRoomTemplate? GetRoomRes(int index, IMapGeneratorConfig config);
}