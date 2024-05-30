using System;
using System.Threading.Tasks;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.map.roomInjectionProcessor;

/// <summary>
/// <para>Time interval for room injection processor</para>
/// <para>时间区间的房间注入处理器</para>
/// </summary>
/// <remarks>
///<para>This processor allows you to specify a time range and allows room generation within the specified time range.</para>
///<para>此处理器允许指定一个时间范围，并在指定的时间范围内允许生成房间。</para>
/// </remarks>
public class
    TimeIntervalRoomInjectorProcessor : RoomInjectionProcessorTemplate<TimeIntervalRoomInjectorProcessor.ConfigData>
{
    public override string GetId()
    {
        return Config.RoomInjectionProcessorId.TimeInterval;
    }

    protected override Task<bool> OnCreateConfigData(RandomNumberGenerator randomNumberGenerator, ConfigData configData)
    {
        if (configData.StartTime == null)
        {
            return Task.FromResult(false);
        }

        if (configData.EndTime == null)
        {
            //If no end time is specified, the default end time is the start time
            //如果未指定结束时间，则默认结束时间为开始时间
            configData.EndTime = configData.StartTime;
        }

        var now = DateTime.Now;
        if (!configData.SpecifiedYear)
        {
            //If no year is specified, it is automatically added to the current year
            //若未指定年份，则自动补充为当前年份
            var nowYear = now.Year;
            configData.StartTime = $"{nowYear}-{configData.StartTime}";
            configData.EndTime = $"{nowYear}-{configData.EndTime}";
        }

        return Task.FromResult(TimeUtils.IsBetweenTimeSpan(now, configData.StartTime, configData.EndTime));
    }


    public class ConfigData
    {
        /// <summary>
        /// <para>Whether to specify a year</para>
        /// <para>是否指定年份</para>
        /// </summary>
        /// <remarks>
        ///<para>If true, then Year can be specified in StartTime and EndTime. The specified year is used to be executed only once in a given year, while configurations that do not specify a year are automatically replenished with the current year (performed annually).</para>
        ///<para>如果为true，那么可以在StartTime和EndTime内指定Year。指定年份被用于在特定的年份仅执行一次，而未指定年份的配置会自动补充为当前年份（每年执行）。</para>
        /// </remarks>
        public bool SpecifiedYear { get; set; }

        /// <summary>
        /// <para>Start time</para>
        /// <para>起始时间</para>
        /// </summary>
        /// <remarks>
        ///<para>If the year is not specified, enter data in the format MM-DD hh:mm:ss. If the year is specified, enter data in the format yyyy-MM-dd hh:mm:ss.</para>
        ///<para>若未指定年份，则可填入格式为MM-dd hh:mm:ss的数据，若指定了年份，那么请填入yyyy-MM-dd hh:mm:ss格式数据。</para>
        /// </remarks>
        public string? StartTime { get; set; }

        /// <summary>
        /// <para>End time</para>
        /// <para>结束时间</para>
        /// </summary>
        /// <remarks>
        ///<para>See StartTime</para>
        ///<para>同StartTime</para>
        /// </remarks>
        public string? EndTime { get; set; }
    }
}