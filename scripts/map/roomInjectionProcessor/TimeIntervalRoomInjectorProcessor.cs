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

        if (configData.DateSpecifiesLevel == null)
        {
            //If no date level is specified, the default is 0
            //若未指定日期等级，则默认为0
            configData.DateSpecifiesLevel = 0;
        }

        var now = DateTime.UtcNow;
        var nowYear = now.Year;
        var nowMonth = now.Month;
        switch (configData.DateSpecifiesLevel)
        {
            case 0:
                //The complete time is specified in the format yyyy/MM/dd hh:mm:ss
                //指定了完整的时间，格式如：yyyy/MM/dd hh:mm:ss
                break;
            case 1:
                //No year is specified. The format is mm/dd hh:mm:ss
                //未指定年份。格式如：MM/dd hh:mm:ss
                configData.StartTime = $"{nowYear}/{configData.StartTime}";
                configData.EndTime = $"{nowYear}/{configData.EndTime}";
                break;
            case 2:
                //No year and month are specified. The format is dd hh:mm:ss
                //未指定年份和月份。格式如：dd hh:mm:ss
                configData.StartTime = $"{nowYear}/{nowMonth}/{configData.StartTime}";
                configData.EndTime = $"{nowYear}/{nowMonth}/{configData.EndTime}";
                break;
            case 3:
                //No year, month, and day are specified. The format is hh:mm:ss
                //未指定年份、月份和日期。格式如：hh:mm:ss
                configData.StartTime = $"{nowYear}/{nowMonth}/{now.Day} {configData.StartTime}";
                configData.EndTime = $"{nowYear}/{nowMonth}/{now.Day} {configData.EndTime}";
                break;
            case 4:
                //No year, month, day, and hour are specified. The format is mm:ss
                //未指定年份、月份、日期和小时。格式如：mm:ss
                configData.StartTime = $"{nowYear}/{nowMonth}/{now.Day} {now.Hour}:{configData.StartTime}";
                configData.EndTime = $"{nowYear}/{nowMonth}/{now.Day} {now.Hour}:{configData.EndTime}";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(configData), "Invalid DateSpecifiesLevel specified.");
        }
        return Task.FromResult(TimeUtils.IsBetweenTimeSpan(now, configData.StartTime, configData.EndTime));
    }


    public class ConfigData
    {
        /// <summary>
        /// <para>Date specifies the level</para>
        /// <para>日期指定的等级</para>
        /// </summary>
        /// <remarks>
        ///<para>Level 0: Specify complete yyyy/MM/DD hh:mm:ss, level 1: yyyy (year automatically uses this year), Level 2: yyyy/MM (year and month automatically uses this month), Level 3: yyyy/MM/DD (automatically uses today), level 4: yyyy/MM/dd hh(The current hour is automatically used)</para>
        ///<para>等级0：指定完整的yyyy/MM/dd hh:mm:ss，等级1：yyyy（年份自动采用今年），等级2：yyyy/MM（年份和月份自动采用本年本月），等级3：yyyy/MM/dd（自动采用今天），等级4：yyyy/MM/dd hh(自动采用当前小时)</para>
        /// </remarks>
        public int? DateSpecifiesLevel { get; set; }

        /// <summary>
        /// <para>Start time</para>
        /// <para>起始时间</para>
        /// </summary>
        public string? StartTime { get; set; }

        /// <summary>
        /// <para>End time</para>
        /// <para>结束时间</para>
        /// </summary>
        public string? EndTime { get; set; }
    }
}