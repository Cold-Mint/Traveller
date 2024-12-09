using System.Collections.Generic;

namespace ColdMint.scripts.map.room;

/// <summary>
/// <para>Room event Manager</para>
/// <para>房间事件管理器</para>
/// </summary>
public static class RoomEventManager
{
    /// <summary>
    /// <para>The event of entering the room</para>
    /// <para>进入房间的事件</para>
    /// </summary>
    private static Dictionary<string, IEnterRoomEventHandler>? _enterRoomEventHandlers;

    /// <summary>
    /// <para>Exit the room event</para>
    /// <para>退出房间的事件</para>
    /// </summary>
    private static Dictionary<string, IExitRoomEventHandler>? _exitRoomEventHandlers;

    /// <summary>
    /// <para>Register the event handler that enters the room</para>
    /// <para>注册进入房间的事件处理器</para>
    /// </summary>
    /// <param name="enterRoomEventHandler"></param>
    /// <returns></returns>
    public static bool RegisterEnterRoomEventHandler(IEnterRoomEventHandler enterRoomEventHandler)
    {
        var id = enterRoomEventHandler.GetId();
        if (_enterRoomEventHandlers != null) return _enterRoomEventHandlers.TryAdd(id, enterRoomEventHandler);
        _enterRoomEventHandlers = new Dictionary<string, IEnterRoomEventHandler>
        {
            {
                id, enterRoomEventHandler
            }
        };
        return true;
    }

    /// <summary>
    /// <para>Gets the event that entered the room</para>
    /// <para>获取进入房间的事件</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static IEnterRoomEventHandler? GetEnterRoomEventHandler(string id)
    {
        return _enterRoomEventHandlers?.TryGetValue(id, out var enterRoomEventHandler) == true
            ? enterRoomEventHandler
            : null;
    }

    /// <summary>
    /// <para>Unregister the room entry event</para>
    /// <para>取消注册进入房间事件</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool UnRegisterEnterRoomEventHandler(string id)
    {
        return _enterRoomEventHandlers?.Remove(id) == true;
    }

    /// <summary>
    /// <para>Unregister for exit room events</para>
    /// <para>取消注册退出房间事件</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool UnRegisterExitRoomEventHandler(string id)
    {
        return _exitRoomEventHandlers?.Remove(id) == true;
    }

    /// <summary>
    /// <para>Gets the event to exit the room</para>
    /// <para>获取退出房间的事件</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static IExitRoomEventHandler? GetExitRoomEventHandler(string id)
    {
        return _exitRoomEventHandlers?.TryGetValue(id, out var exitRoomEventHandler) == true
            ? exitRoomEventHandler
            : null;
    }

    /// <summary>
    /// <para>Sign up to exit the room's event handler</para>
    /// <para>注册退出房间的事件处理器</para>
    /// </summary>
    /// <param name="exitRoomEventHandler"></param>
    /// <returns></returns>
    public static bool RegisterExitRoomEventHandler(IExitRoomEventHandler exitRoomEventHandler)
    {
        var id = exitRoomEventHandler.GetId();
        if (_exitRoomEventHandlers != null) return _exitRoomEventHandlers.TryAdd(id, exitRoomEventHandler);
        _exitRoomEventHandlers = new Dictionary<string, IExitRoomEventHandler>
        {
            {
                id, exitRoomEventHandler
            }
        };
        return true;
    }
}