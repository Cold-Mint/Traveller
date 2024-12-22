using System.Threading.Tasks;

namespace ColdMint.scripts.console;

public interface ICommand
{
    /// <summary>
    /// <para>Name</para>
    /// <para>名称</para>
    /// </summary>
    string Name { get; }

    /// <summary>
    /// <para>Suggest</para>
    /// <para>命令的输入建议</para>
    /// </summary>
    string[][] Suggest { get; }

    /// <summary>
    /// <para>Execute</para>
    /// <para>执行命令</para>
    /// </summary>
    /// <param name="args">
    ///<para>args</para>
    ///<para>参数</para>
    /// </param>
    /// <returns></returns>
    Task<bool> Execute(string[] args);
}