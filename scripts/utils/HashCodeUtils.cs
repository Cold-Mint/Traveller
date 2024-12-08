using System.Linq;

namespace ColdMint.scripts.utils;

/// <summary>
/// <para>Hash code utils</para>
/// <para>哈希码工具</para>
/// </summary>
public static class HashCodeUtils
{
    /// <summary>
    /// <para>Gets the hash code for a string</para>
    /// <para>获取字符串的哈希码</para>
    /// </summary>
    /// <param name="str">
    ///<para>The input string returns a fixed hash code</para>
    ///<para>输入的字符串，返回固定的哈希码</para>
    /// </param>
    /// <returns></returns>
    public static uint GetFixedHashCode(string str)
    {
        //Turn off overflow checking to improve performance
        //关闭溢出检查，以提高性能
        unchecked
        {
            return str.Aggregate(2_166_136_261, (current, c) => (current ^ c) * 16_777_619);
        }
    }
}