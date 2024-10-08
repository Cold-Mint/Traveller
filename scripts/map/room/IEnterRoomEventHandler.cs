using Godot;

namespace ColdMint.scripts.map.room;

/// <summary>
/// <para>Enter the event handler in the room</para>
/// <para>进入房间的事件处理器</para>
/// </summary>
public interface IEnterRoomEventHandler
{
    /// <summary>
    /// <para>Get ID</para>
    /// <para>获取ID</para>
    /// </summary>
    /// <returns></returns>
    string GetId();
    
    /// <summary>
    /// <para>When entering the room</para>
    /// <para>当进入房间时</para>
    /// </summary>
    /// <param name="playerRoomVisitCount">
    ///<para>The number of times the player visits the room, 1 is the first visit.</para>
    ///<para>玩家访问房间的次数，为1则代表首次访问。</para>
    /// </param>
    /// <param name="node"></param>
    /// <param name="room"></param>
    void OnEnterRoom(int playerRoomVisitCount,Node node,Room room);
}