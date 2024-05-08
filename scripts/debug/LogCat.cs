using System;
using System.Text;
using Godot;

namespace ColdMint.scripts.debug;

public static class LogCat
{
    private static readonly StringBuilder StringBuilder = new StringBuilder();


    private static StringBuilder HandleMessage(string message)
    {
        StringBuilder.Clear();
        StringBuilder.Append(DateTime.Now.ToString("yyyy-M-d HH:mm:ss |"));
        StringBuilder.Append(TranslationServer.Translate(message));
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
        GD.Print(HandleMessage(message));
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
        GD.PrintErr(HandleMessage(message));
    }

    public static void LogErrorWithFormat(string message, params object?[] args)
    {
        GD.PrintErr(string.Format(HandleMessage(message).ToString(), args));
    }
    

    public static void LogWithFormat(string message, params object?[] args)
    {
        GD.Print(string.Format(HandleMessage(message).ToString(), args));
    }

    public static void LogError(Exception e)
    {
        GD.PrintErr(HandleMessage(e.Message).Append('\n').Append(e.StackTrace));
    }
}