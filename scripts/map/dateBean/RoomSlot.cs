using System;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.map.dateBean;

/// <summary>
/// <para>RoomSlot</para>
/// <para>槽</para>
/// </summary>
public class RoomSlot
{
    /// <summary>
    /// <para>Return True if the slot already matches</para>
    /// <para>如果此槽已匹配，那么返回True</para>
    /// </summary>
    public bool Matched { get; set; }

    /// <summary>
    /// <para>The starting position of the room slot</para>
    /// <para>房间插槽的开始位置</para>
    /// </summary>
    ///<remarks>
    ///<para>As opposed to a tile map. Convert to local location please call <see cref="TileMap.MapToLocal"/></para>
    ///<para>相对于瓦片地图而言的。转换为本地位置请调用<see cref="TileMap.MapToLocal"/></para>
    /// </remarks>
    public Vector2I StartPosition { get; set; }


    /// <summary>
    /// <para>The end position of the room slot</para>
    /// <para>房间插槽的结束位置</para>
    /// </summary>
    ///<remarks>
    ///<para>As opposed to a tile map. Convert to local location please call <see cref="TileMap.MapToLocal"/></para>
    ///<para>相对于瓦片地图而言的。转换为本地位置请调用<see cref="TileMap.MapToLocal"/></para>
    /// </remarks>
    public Vector2I EndPosition { get; set; }

    /// <summary>
    /// <para>Is it a horizontal slot</para>
    /// <para>是水平方向的槽吗</para>
    /// </summary>
    public bool IsHorizontal => StartPosition.Y == EndPosition.Y;


    /// <summary>
    /// <para>Distance from the midpoint of the slot to the midpoint of the room (tile size)</para>
    /// <para>此槽的中点到房间中点的距离（瓦片尺寸）</para>
    /// </summary>
    /// <remarks>
    ///<para>Element 1 represents left and right, element 2 represents up and down</para>
    ///<para>元素1，代表左右，元素2代表上下</para>
    /// </remarks>
    public CoordinateUtils.OrientationDescribe[]? DistanceToMidpointOfRoom { get; set; }

    public int Length =>
        Math.Max(Math.Abs(EndPosition.X - StartPosition.X), Math.Abs(EndPosition.Y - StartPosition.Y)) + 1;
}