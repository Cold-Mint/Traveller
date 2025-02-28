﻿using System;
using System.Collections.Generic;
using System.Text;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.debug;

public static class LogCat
{
    public static class LogLabel
    {
        /// <summary>
        /// <para>Default log label</para>
        /// <para>默认的日志标签</para>
        /// </summary>
        public const string Default = "Default";

        /// <summary>
        /// <para>Patchwork Room Placement Strategy</para>
        /// <para>放置房间策略</para>
        /// </summary>
        public const string PatchworkRoomPlacementStrategy = "PatchworkRoomPlacementStrategy";

        /// <summary>
        /// <para>ItemContainerDisplay</para>
        /// <para>项目容器显示器</para>
        /// </summary>
        public const string ItemContainerDisplay = "ItemContainerDisplay";

        /// <summary>
        /// <para>LookForWeaponProcessor</para>
        /// <para>查找武器处理器</para>
        /// </summary>
        public const string LookForWeaponProcessor = "LookForWeaponProcessor";

        /// <summary>
        /// <para>PatrolStateProcessor</para>
        /// <para>巡逻状态处理器</para>
        /// </summary>
        public const string PatrolStateProcessor = "PatrolStateProcessor";

        /// <summary>
        /// <para>CampManager</para>
        /// <para>阵营管理器</para>
        /// </summary>
        public const string CampManager = "CampManager";

        /// <summary>
        /// <para>State context</para>
        /// <para>状态上下文</para>
        /// </summary>
        public const string StateContext = "StateContext";

        /// <summary>
        /// <para>StateMachineTemplate</para>
        /// <para>状态机模板</para>
        /// </summary>
        public const string StateMachineTemplate = "StateMachineTemplate";

        /// <summary>
        /// <para>Pursuit enemy processor</para>
        /// <para>追击敌人处理器</para>
        /// </summary>
        public const string ChaseStateProcessor = "ChaseStateProcessor";

        /// <summary>
        /// <para>BubbleMarker</para>
        /// <para>气泡标记</para>
        /// </summary>
        public const string BubbleMarker = "BubbleMarker";

        /// <summary>
        /// <para>Control console</para>
        /// <para>控制台</para>
        /// </summary>
        public const string Console = "Console";

        /// <summary>
        /// <para>Mod Loader</para>
        /// <para>模组加载器</para>
        /// </summary>
        public const string ModLoader = "ModLoader";

        /// <summary>
        /// <para>Room</para>
        /// <para>房间</para>
        /// </summary>
        public const string Room = "Room";

        /// <summary>
        /// <para>ItemSpawn</para>
        /// <para>物品生成器</para>
        /// </summary>
        public const string ItemSpawn = "ItemSpawn";

        /// <summary>
        /// <para>MeleeWeapon</para>
        /// <para>近战武器</para>
        /// </summary>
        public const string MeleeWeapon = "MeleeWeapon";
    }


    /// <summary>
    /// <para>Information log level</para>
    /// <para>信息日志等级</para>
    /// </summary>
    public const int InfoLogLevel = 0;

    /// <summary>
    /// <para>Warning level</para>
    /// <para>警告等级</para>
    /// </summary>
    public const int WarningLogLevel = 1;

    /// <summary>
    /// <para>Error log level</para>
    /// <para>错误日志等级</para>
    /// </summary>
    public const int ErrorLogLevel = 2;

    /// <summary>
    ///<para>Disable all logs</para>
    ///<para>禁用所有日志</para>
    /// </summary>
    public const int DisableAllLogLevel = int.MaxValue;

    private static int _minLogLevel = InfoLogLevel;

    /// <summary>
    /// <para>Set the minimum log level</para>
    /// <para>设置最小日志等级</para>
    /// </summary>
    /// <remarks>
    ///<para>Set a value for it so that LogCat only prints logs with a level higher than the set value.</para>
    ///<para>为其设置一个值，使LogCat仅打印等级高于设定值的日志。</para>
    /// </remarks>
    public static int MinLogLevel
    {
        get => _minLogLevel;
        set => _minLogLevel = value;
    }


    private static readonly StringBuilder StringBuilder = new();

    /// <summary>
    /// <para>Disabled log label</para>
    /// <para>禁用的日志标签</para>
    /// </summary>
    private static HashSet<string> DisabledLogLabels { get; } = [];


    /// <summary>
    /// <para>Disable log Label</para>
    /// <para>禁用某个日志标签</para>
    /// </summary>
    /// <param name="label">
    ///<para>label</para>
    ///<para>标签名称</para>
    /// </param>
    /// <returns>
    ///<para>Returns whether the function is disabled successfully</para>
    ///<para>返回是否禁用成功</para>
    /// </returns>
    public static bool DisableLogLabel(string label)
    {
        return DisabledLogLabels.Add(label);
    }

    /// <summary>
    /// <para>Whether a label is enabled</para>
    /// <para>某个标签是否处于启用状态</para>
    /// </summary>
    /// <param name="label">
    ///<para>label</para>
    ///<para>标签名称</para>
    /// </param>
    /// <returns>
    ///<para>Whether enabled</para>
    ///<para>是否处于启用</para>
    /// </returns>
    public static bool IsEnabledLogLabel(string label)
    {
        return !DisabledLogLabels.Contains(label);
    }

    /// <summary>
    /// <para>EnableLogLabel</para>
    /// <para>启用某个日志标签</para>
    /// </summary>
    /// <param name="label">
    ///<para>label</para>
    /// <para>标签名称</para>
    /// </param>
    /// <returns>
    ///<para>Returns whether the function is enabled successfully</para>
    ///<para>返回是否启用成功</para>
    /// </returns>
    public static bool EnableLogLabel(string label)
    {
        return DisabledLogLabels.Remove(label);
    }

    private static StringBuilder HandleMessage(int level, string message, string label)
    {
        StringBuilder.Clear();
        switch (level)
        {
            case InfoLogLevel:
                StringBuilder.Append("INFO");
                break;
            case WarningLogLevel:
                StringBuilder.Append("WARNING");
                break;
            case ErrorLogLevel:
                StringBuilder.Append("ERROR");
                break;
            default:
                StringBuilder.Append("INFO");
                break;
        }

        StringBuilder.Append(DateTime.UtcNow.ToString(" (K)yyyy-MM-d hh:mm:ss.fff "));
        StringBuilder.Append(label);
        StringBuilder.Append(" :");
        var key = $"log_{message}";
        var translationResult = TranslationServerUtils.Translate(key);
        StringBuilder.Append(translationResult == key ? message : translationResult);
        return StringBuilder;
    }

    /// <summary>
    /// <para>Print log</para>
    /// <para>打印日志</para>
    /// </summary>
    /// <param name="message">
    ///<para>message</para>
    ///<para>消息</para>
    /// <para>This message supports localized output, assuming there is already a translation key, Hello = 你好, passing hello will output 你好.</para>
    /// <para>这个消息支持本地化输出，假设已存在翻译key，Hello = 你好，传入Hello则会输出你好。</para>
    /// </param>
    /// <param name="label">
    /// </param>
    public static void Log(string message, string label = LogLabel.Default)
    {
        PrintLog(InfoLogLevel, HandleMessage(InfoLogLevel, message, label).ToString(), label);
    }

    private static void PrintLog(int level, string concreteLog, string label)
    {
        if (!IsEnabledLogLabel(label))
        {
            return;
        }

        if (_minLogLevel > InfoLogLevel)
        {
            return;
        }

        switch (level)
        {
            case WarningLogLevel:
                GD.Print(concreteLog);
                break;
            case ErrorLogLevel:
                GD.PrintErr(concreteLog);
                break;
            default:
                GD.Print(concreteLog);
                break;
        }
    }

    /// <summary>
    /// <para>Print error log</para>
    /// <para>打印错误日志</para>
    /// </summary>
    /// <param name="message">
    ///<para>message</para>
    ///<para>消息</para>
    /// <para>This message supports localized output, assuming there is already a translation key, Hello = 你好, passing hello will output 你好.</para>
    /// <para>这个消息支持本地化输出，假设已存在翻译key，Hello = 你好，传入Hello则会输出你好。</para>
    /// </param>
    /// <param name="label"></param>
    public static void LogError(string message, string label = LogLabel.Default)
    {
        PrintLog(ErrorLogLevel, HandleMessage(ErrorLogLevel, message, label).ToString(), label);
    }

    public static void LogWarning(string message, string label = LogLabel.Default)
    {
        PrintLog(WarningLogLevel, HandleMessage(WarningLogLevel, message, label).ToString(), label);
    }

    public static void LogErrorWithFormat(string message, string label, params object?[] args)
    {
        PrintLog(ErrorLogLevel, string.Format(HandleMessage(ErrorLogLevel, message, label).ToString(), args), label);
    }


    public static void LogWithFormat(string message, string label, params object?[] args)
    {
        PrintLog(InfoLogLevel, string.Format(HandleMessage(InfoLogLevel, message, label).ToString(), args), label);
    }

    public static void LogWarningWithFormat(string message, string label, params object?[] args)
    {
        PrintLog(WarningLogLevel, string.Format(HandleMessage(WarningLogLevel, message, label).ToString(), args),
            label);
    }

    /// <summary>
    /// <para>This method is called when an exception is caught</para>
    /// <para>当捕获异常后调用此方法</para>
    /// </summary>
    /// <param name="e"></param>
    /// <param name="label"></param>
    public static void WhenCaughtException(Exception e, string label = LogLabel.Default)
    {
        if (!IsEnabledLogLabel(label))
        {
            return;
        }

        //Log an exception here or send it to the server.
        //请在这里记录异常或将异常发送至服务器。
        PrintLog(ErrorLogLevel,
            HandleMessage(ErrorLogLevel, e.Message, label).Append('\n').Append(e.StackTrace).ToString(), label);
    }
}