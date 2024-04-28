namespace ColdMint.scripts.map.interfaces;

public interface IRoomTemplate
{
    /// <summary>
    /// <para>The asset path of the room must be available</para>
    /// <para>必须可获得房间的资产路径</para>
    /// </summary>
    string RoomResPath { get; }

    /// <summary>
    /// <para>Whether this room template can still be used</para>
    /// <para>这个房间模板是否还能使用</para>
    /// </summary>
    bool CanUse { get; }

    /// <summary>
    /// <para>The maximum number of times a room template is used</para>
    /// <para>房间模板的最大使用次数</para>
    /// </summary>
    int MaxNumber { get; set; }
    
    
    /// <summary>
    /// <para>AddUsedNumber</para>
    /// <para>添加使用次数</para>
    /// </summary>
    void AddUsedNumber();
    
    /// <summary>
    /// <para>Times used</para>
    /// <para>已使用次数</para>
    /// </summary>
    int UsedNumber { get; }
}