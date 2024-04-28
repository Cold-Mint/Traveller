using ColdMint.scripts.map.interfaces;
using Godot;

namespace ColdMint.scripts.map.room;

/// <summary>
/// <para>The room template factory is used to generate room templates</para>
/// <para>房间模板工厂用于生成房间模板</para>
/// </summary>
public static class RoomFactory
{
    /// <summary>
    /// <para>CreateRoom</para>
    /// <para>创建房间模板</para>
    /// </summary>
    /// <param name="resPath"></param>
    /// <returns></returns>
    public static IRoom CreateRoom(string resPath)
    {
        var room = new Room
        {
            RoomScene = GD.Load<PackedScene>(resPath)
        };
        return room;
    }
}