using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ColdMint.scripts.utils;

public class Md5Utils
{
    /// <summary>
    /// <para>Calculates the Md5 value of the file</para>
    /// <para>计算文件的Md5值</para>
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetFileMd5(string filePath)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    /// <summary>
    /// <para>Calculates the Md5 value of the string</para>
    /// <para>计算字符串的Md5值</para>
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string GetStringMd5(string str)
    {
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}