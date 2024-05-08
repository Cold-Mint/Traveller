namespace ColdMint.scripts.map.interfaces;

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