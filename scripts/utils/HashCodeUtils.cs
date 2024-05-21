using System.Linq;

namespace ColdMint.scripts.utils;

public class HashCodeUtils
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
        unchecked
        {
            return str.Aggregate(2166136261, (current, c) => (current ^ c) * 16777619);
        }
    }
}