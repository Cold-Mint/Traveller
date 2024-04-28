using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ColdMint.scripts.debug;
using ColdMint.scripts.map.dateBean;
using Godot;
using Godot.Collections;

namespace ColdMint.scripts.utils;

public static class TileMapUtils
{
    /// <summary>
    /// <para>Get the layer number corresponding to LayerName in TileMap</para>
    /// <para>在TileMap内获取LayerName所对应的图层序号</para>
    /// </summary>
    /// <param name="tileMap"></param>
    /// <param name="layerName"></param>
    /// <returns>
    ///<para>− 1 is returned after obtaining failure</para>
    ///<para>获取失败返回-1</para>
    /// </returns>
    public static int GetTileMapLayer(TileMap tileMap, string layerName)
    {
        var count = tileMap.GetLayersCount();
        for (var i = 0; i < count; i++)
        {
            var currentLayerName = tileMap.GetLayerName(i);
            if (currentLayerName == layerName)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// <para>Resolve the slots in the room layer</para>
    /// <para>在房间图层内解析槽位</para>
    /// </summary>
    /// <param name="tileMap"></param>
    /// <param name="layerName"></param>
    /// <remarks>
    ///<para>This method was written by Github Copilot.</para>
    ///<para>此方法由Github Copilot编写。</para>
    /// </remarks>
    /// <returns></returns>
    public static RoomSlot[] GetRoomSlots(TileMap tileMap, string layerName)
    {
        var layer = GetTileMapLayer(tileMap, layerName);
        if (layer == -1)
        {
            return null;
        }

        var cells = tileMap.GetUsedCells(layer);
        if (cells.Count == 0)
        {
            return null;
        }

        //保存上一个瓦片
        var roomSlots = new List<RoomSlot>();
        var currentSlot = new RoomSlot();
        for (int i = 0; i < cells.Count; i++)
        {
            var currentCell = cells[i];
            if (i == 0)
            {
                //首次循环时初始化瓦片，设置起始位置和结束位置
                currentSlot.StartPosition = currentCell;
                currentSlot.EndPosition = currentCell;
                continue;
            }

            var distance = currentSlot.EndPosition - currentSlot.StartPosition;
            LogCat.Log("距离从" + currentSlot.StartPosition + "到" + currentSlot.EndPosition + "为" + distance);
            if (distance == Vector2I.Zero)
            {
                //原点可匹配临近瓦片，不需要方向判断
                var formCellDistance = currentCell - currentSlot.EndPosition;
                if (Math.Abs(formCellDistance.X) == 1 || Math.Abs(formCellDistance.Y) == 1)
                {
                    //方块是相邻的(将终点设置到此方块)
                    LogCat.Log("添加与原点" + currentSlot.EndPosition + "相邻的坐标" + currentCell);
                    currentSlot.EndPosition = currentCell;
                    continue;
                }
            }

            if (distance.X == 0)
            {
                //垂直方向
                var formCellDistanceEnd = currentCell - currentSlot.EndPosition;
                LogCat.Log("更新垂直：" + currentCell + "到终点" + currentSlot.EndPosition + "起点" + currentSlot.StartPosition +
                           "为" + formCellDistanceEnd);
                if (formCellDistanceEnd.X != 0)
                {
                    //新瓦片不与终点和起点在一条直线上
                    continue;
                }

                //方块是相邻的(将终点设置到此方块)
                if (Math.Abs(formCellDistanceEnd.Y) == 1)
                {
                    currentSlot.EndPosition = currentCell;
                    continue;
                }


                var formCellDistanceStart = currentCell - currentSlot.StartPosition;
                LogCat.Log("更新垂直：" + currentCell + "到终点" + currentSlot.StartPosition + "终点" +
                           currentSlot.StartPosition +
                           "为" + formCellDistanceEnd);

                //方块是相邻的(将终点设置到此方块)
                if (Math.Abs(formCellDistanceStart.Y) == 1)
                {
                    currentSlot.StartPosition = currentCell;
                    continue;
                }
            }

            if (distance.Y == 0)
            {
                //水平方向
                var formCellDistanceEnd = currentCell - currentSlot.EndPosition;
                LogCat.Log("更新水平：" + currentCell + "到" + currentSlot.EndPosition + "为" + formCellDistanceEnd);
                if (formCellDistanceEnd.Y != 0)
                {
                    continue;
                }

                //方块是相邻的(将终点设置到此方块)
                if (Math.Abs(formCellDistanceEnd.X) == 1)
                {
                    currentSlot.EndPosition = currentCell;
                    continue;
                }
                
                var formCellDistanceStart = currentCell - currentSlot.StartPosition;
                LogCat.Log("更新水平：" + currentCell + "到终点" + currentSlot.StartPosition + "终点" +
                           currentSlot.StartPosition +
                           "为" + formCellDistanceEnd);

                //方块是相邻的(将终点设置到此方块)
                if (Math.Abs(formCellDistanceStart.X) == 1)
                {
                    currentSlot.StartPosition = currentCell;
                    continue;
                }
            }

            //如果不是临近方块，那么提交槽位
            LogCat.Log("新方块" + currentCell + "与槽位" + currentSlot.StartPosition + "到" + currentSlot.EndPosition +
                       "不匹配。");
            roomSlots.Add(currentSlot);
            currentSlot = new RoomSlot();
            currentSlot.StartPosition = currentCell;
            currentSlot.EndPosition = currentCell;
        }

        //添加没有提交的值
        roomSlots.Add(currentSlot);
        return roomSlots.ToArray();
    }


    // public static RoomSlot[] GetRoomSlots(TileMap tileMap, string layerName)
    // {
    //     var layer = GetTileMapLayer(tileMap, layerName);
    //     if (layer == -1)
    //     {
    //         return null;
    //     }
    //
    //     var cells = tileMap.GetUsedCells(layer).OrderBy(c => c.Y).ThenBy(c => c.X).ToList();
    //     var roomSlots = new List<RoomSlot>();
    //     RoomSlot currentSlot = null;
    //
    //     for (int i = 0; i < cells.Count - 1; i++)
    //     {
    //         var currentCell = cells[i];
    //         var nextCell = cells[i + 1];
    //
    //         // Check if the current cell and the next cell are consecutive
    //         if ((currentCell.X == nextCell.X && Math.Abs(currentCell.Y - nextCell.Y) == 1) ||
    //             (currentCell.Y == nextCell.Y && Math.Abs(currentCell.X - nextCell.X) == 1))
    //         {
    //             if (currentSlot == null)
    //             {
    //                 currentSlot = new RoomSlot { StartPosition = currentCell, EndPosition = nextCell };
    //             }
    //             else
    //             {
    //                 currentSlot.EndPosition = nextCell;
    //             }
    //         }
    //         else
    //         {
    //             if (currentSlot != null)
    //             {
    //                 roomSlots.Add(currentSlot);
    //                 currentSlot = null;
    //             }
    //         }
    //     }
    //
    //     // Add the last slot if it's not null
    //     if (currentSlot != null)
    //     {
    //         roomSlots.Add(currentSlot);
    //     }
    //     return roomSlots.ToArray();
    // }
}