using System.Threading.Tasks;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.console;

/// <summary>
/// <para>Command executor</para>
/// <para>命令执行器</para>
/// </summary>
public static class CommandExecutor
{
    /// <summary>
    /// <para>ExecuteCommand</para>
    /// <para>执行命令</para>
    /// </summary>
    /// <param name="commandString"></param>
    /// <returns>
    ///<para>Returns whether the execution was successful</para>
    ///<para>返回是否执行成功</para>
    /// </returns>
    public static async Task<bool> ExecuteCommandAsync(string commandString)
    {
        ConsoleGui.Instance?.Print(TranslationServerUtils.TranslateWithFormat("log_command_execution", commandString));
        if (string.IsNullOrEmpty(commandString))
        {
            ExecutedFailure(commandString);
            return false;
        }

        var arguments = commandString.Split(" ");
        if (arguments.Length == 0)
        {
            ExecutedFailure(commandString);
            return false;
        }

        var commandName = arguments[0];
        var command = CommandManager.GetCommand(commandName);
        if (command == null)
        {
            ExecutedFailure(commandString);
            return false;
        }

        var result = await command.Execute(new CommandArgs(arguments));
        if (result)
        {
            ConsoleGui.Instance?.Print(
                TranslationServerUtils.TranslateWithFormat("log_command_executed_success", commandString));
        }
        else
        {
            ExecutedFailure(commandString);
        }

        return result;
    }

    /// <summary>
    /// <para>ExecutedFailure</para>
    /// <para>执行失败</para>
    /// </summary>
    /// <param name="commandString"></param>
    private static void ExecutedFailure(string commandString)
    {
        ConsoleGui.Instance?.Print(
            TranslationServerUtils.TranslateWithFormat("log_command_executed_failure", commandString));
    }
}