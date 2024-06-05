namespace ColdMint.scripts.utils;

/// <summary>
/// <para>StrUtils</para>
/// <para>字符串工具</para>
/// </summary>
public static class StrUtils
{

    /// <summary>
    /// <para>Gets the last line of a string</para>
    /// <para>获取某个字符串的最后一行</para>
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string GetLastLine(string str)
    {
        var index = str.LastIndexOf('\n');
        return index == -1 ? str : str[(index + 1)..];
    }
}