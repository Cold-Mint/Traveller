using System;
using ColdMint.scripts.debug;

namespace ColdMint.scripts.utils;

public static class TimeUtils
{
    /// <summary>
    /// <para>Determines whether the specified time is within the specified range</para>
    /// <para>判断指定的时间是否在指定的范围</para>
    /// </summary>
    /// <param name="dateTime">
    /// <para>The value is a string in the format yyyy-MM-dd hh:mm:ss</para>
    /// <para>指定时间，字符串类型，形如：yyyy-MM-dd hh:mm:ss</para>
    /// </param>
    /// <param name="startTime">
    ///<para>The value is a string of characters in the format yyyy-MM-dd hh:mm:ss</para>
    /// <para>开始时间，字符串类型，形如：yyyy-MM-dd hh:mm:ss</para>
    /// </param>
    /// <param name="endTime">
    /// <para>End time The value is a string in the format yyyy-MM-dd hh:mm:ss</para>
    ///<para>结束时间，字符串类型，形如：yyyy-MM-dd hh:mm:ss</para>
    /// </param>
    /// <returns></returns>
    public static bool IsBetweenTimeSpan(DateTime dateTime, string startTime, string endTime)
    {
        var dtStartTime = Convert.ToDateTime(startTime);
        var dtEndTime = Convert.ToDateTime(endTime);
        var compNum1 = DateTime.Compare(dateTime, dtStartTime);
        var compNum2 = DateTime.Compare(dateTime, dtEndTime);
        var result = compNum1 >= 0 && compNum2 <= 0;
        LogCat.LogWithFormat("time_range_debug", dateTime, dtStartTime, dtEndTime, result);
        return result;
    }
}