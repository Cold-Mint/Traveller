using Godot;

namespace ColdMint.scripts.map.room;

/// <summary>
/// <para>Exit the room's processor</para>
/// <para>退出房间的处理器</para>
/// </summary>
public interface IExitRoomEventHandler
{
    /// <summary>
    /// <para>Get ID</para>
    /// <para>获取ID</para>
    /// </summary>
    /// <returns></returns>
    string GetId();
    
    /// <summary>
    /// <para>When exiting the room</para>
    /// <para>当退出房间时</para>
    /// </summary>
    /// <param name="node"></param>
    /// <param name="room"></param>
    void OnExitRoom(Node node,Room room);
}