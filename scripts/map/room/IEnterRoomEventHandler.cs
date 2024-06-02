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
    /// <param name="node"></param>
    /// <param name="room"></param>
    void OnEnterRoom(Node node,Room room);
}