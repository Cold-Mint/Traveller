using System.Collections.Generic;
using System.IO;
using ColdMint.scripts.utils;
using Godot;
using FileAccess = Godot.FileAccess;

namespace ColdMint.scripts.map.room;

/// <summary>
/// <para>The room template factory is used to generate room templates</para>
/// <para>房间模板工厂用于生成房间模板</para>
/// </summary>
public static class RoomFactory
{
    /// <summary>
    /// <para>A room template sets a path to a room resource</para>
    /// <para>房间模板集转房间资源路径</para>
    /// </summary>
    /// <param name="roomTemplateSet"></param>
    /// <returns>
    /// <para>Returned value Checked for the existence of the file.</para>
    /// <para>返回值已检验文件是否存在。</para>
    /// </returns>
    public static string[] RoomTemplateSetToRoomRes(string[] roomTemplateSet)
    {
        var resList = new List<string>();
        foreach (var roomTemplate in roomTemplateSet)
        {
            var roomTemplatePath = ResUtils.GetRunTimeResPath(roomTemplate);
            //Detects whether it is a folder
            //检测是否为文件夹
            if (DirAccess.DirExistsAbsolute(roomTemplatePath))
            {
                using var dir = DirAccess.Open(roomTemplatePath);
                if (dir != null)
                {
                    dir.ListDirBegin();
                    var fileName = dir.GetNext();
                    while (!string.IsNullOrEmpty(fileName))
                    {
                        if (!dir.CurrentIsDir())
                        {
                            resList.Add(Path.Join(roomTemplatePath, fileName));
                        }

                        fileName = dir.GetNext();
                    }
                }
            }

            if (FileAccess.FileExists(roomTemplatePath))
            {
                resList.Add(roomTemplatePath);
            }
        }

        return resList.ToArray();
    }


    /// <summary>
    /// <para>CreateRoom</para>
    /// <para>创建房间模板</para>
    /// </summary>
    /// <param name="resPath">
    ///<para>resources path</para>
    ///<para>资源路径</para>
    /// </param>
    /// <param name="enterRoomEventHandlerId">
    ///<para>The ID of the event handler when entering the room</para>
    ///<para>进入房间时的事件处理器ID</para>
    /// </param>
    /// <param name="exitRoomEventHandlerId">
    ///<para>Event handler ID when exiting the room</para>
    ///<para>退出房间时的事件处理器ID</para>
    /// </param>
    /// <returns></returns>
    public static Room? CreateRoom(string resPath, string? enterRoomEventHandlerId = null,
        string? exitRoomEventHandlerId = null)
    {
        //If the file does not exist, null is returned
        //如果文件不存在，则返回null
        var exists = FileAccess.FileExists(resPath);
        if (!exists)
        {
            return null;
        }

        var room = new Room
        {
            RoomScene = GD.Load<PackedScene>(ResUtils.GetEditorResPath(resPath)),
            EnterRoomEventHandlerId = enterRoomEventHandlerId,
            ExitRoomEventHandlerId = exitRoomEventHandlerId
        };
        return room;
    }
}