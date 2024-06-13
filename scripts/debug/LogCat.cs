using System;
using System.Text;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.debug;

public static class LogCat
{
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

    private static readonly StringBuilder StringBuilder = new StringBuilder();


    private static StringBuilder HandleMessage(int level, string message)
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

        StringBuilder.Append(DateTime.Now.ToString(" yyyy-M-d HH:mm:ss : "));
        StringBuilder.Append(TranslationServerUtils.Translate($"log_{message}"));
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
    public static void Log(string message)
    {
        if (_minLogLevel > InfoLogLevel)
        {
            return;
        }

        GD.Print(HandleMessage(InfoLogLevel, message));
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
    public static void LogError(string message)
    {
        if (_minLogLevel > ErrorLogLevel)
        {
            return;
        }

        GD.PrintErr(HandleMessage(ErrorLogLevel, message));
    }

    public static void LogWarning(string message)
    {
        if (_minLogLevel > WarningLogLevel)
        {
            return;
        }

        GD.Print(HandleMessage(WarningLogLevel, message));
    }

    public static void LogErrorWithFormat(string message, params object?[] args)
    {
        if (_minLogLevel > ErrorLogLevel)
        {
            return;
        }

        GD.PrintErr(string.Format(HandleMessage(ErrorLogLevel, message).ToString(), args));
    }


    public static void LogWithFormat(string message, params object?[] args)
    {
        if (_minLogLevel > InfoLogLevel)
        {
            return;
        }

        GD.Print(string.Format(HandleMessage(InfoLogLevel, message).ToString(), args));
    }

    public static void LogWarningWithFormat(string message, params object?[] args)
    {
        if (_minLogLevel > InfoLogLevel)
        {
            return;
        }

        GD.Print(string.Format(HandleMessage(WarningLogLevel, message).ToString(), args));
    }

    /// <summary>
    /// <para>This method is called when an exception is caught</para>
    /// <para>当捕获异常后调用此方法</para>
    /// </summary>
    /// <param name="e"></param>
    public static void WhenCaughtException(Exception e)
    {
        //Log an exception here or send it to the server.
        //请在这里记录异常或将异常发送至服务器。
        GD.PrintErr(HandleMessage(ErrorLogLevel, e.Message).Append('\n').Append(e.StackTrace));
    }
}