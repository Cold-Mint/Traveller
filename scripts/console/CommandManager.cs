using System.Collections.Generic;

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
    private static Dictionary<string, ICommand> _commands = new();

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
        return _commands.TryAdd(command.Name, command);
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
        return _commands.Remove(command.Name);
    }

    /// <summary>
    /// <para>GetCommand</para>
    /// <para>获取某个命令</para>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static ICommand? GetCommand(string name)
    {
        return _commands.GetValueOrDefault(name);
    }
}