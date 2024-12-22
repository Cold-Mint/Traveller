using System.Collections.Generic;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.console;

/// <summary>
/// <para>CommandManager</para>
/// <para>命令管理器</para>
/// </summary>
/// <remarks>
///<para>Add or remove commands in the command manager.</para>
///<para>在命令管理器内添加或移除命令。</para>
/// </remarks>
public class CommandManager
{
    private static readonly Dictionary<string, ICommand> Commands = new();
    private static readonly List<string> CommandKeys = [];

    /// <summary>
    /// <para>RegisterCommand</para>
    /// <para>注册命令</para>
    /// </summary>
    /// <param name="command">
    ///<para>command</para>
    ///<para>命令</para>
    /// </param>
    /// <returns></returns>
    public static bool RegisterCommand(ICommand command)
    {
        var lowerName = command.Name.ToLowerInvariant();
        var result = Commands.TryAdd(lowerName, command);
        if (result)
        {
            CommandKeys.Add(lowerName);
        }

        return result;
    }

    /// <summary>
    /// <para>GetSuggest</para>
    /// <para>获取建议</para>
    /// </summary>
    /// <param name="commandName"></param>
    /// <returns></returns>
    public static string[] GetSuggest(string commandName)
    {
        return SuggestUtils.ScreeningSuggestion(CommandKeys, commandName);
    }

    /// <summary>
    /// <para>UnregisterCommand</para>
    /// <para>注销命令</para>
    /// </summary>
    /// <param name="command">
    ///<para>command</para>
    ///<para>命令</para>
    /// </param>
    /// <returns></returns>
    public static bool UnregisterCommand(ICommand command)
    {
        var lowerName = command.Name.ToLowerInvariant();
        var result = CommandKeys.Remove(lowerName);
        if (result)
        {
            CommandKeys.Remove(lowerName);
        }

        return result;
    }

    /// <summary>
    /// <para>GetCommand</para>
    /// <para>获取某个命令</para>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static ICommand? GetCommand(string name)
    {
        var lowerName = name.ToLowerInvariant();
        return string.IsNullOrEmpty(lowerName) ? null : Commands.GetValueOrDefault(lowerName);
    }
}