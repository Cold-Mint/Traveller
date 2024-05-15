using System;

namespace ColdMint.scripts.utils;

public class GuidUtils
{
    /// <summary>
    /// <para>Get the new GUID</para>
    /// <para>获取新的GUID</para>
    /// </summary>
    /// <returns></returns>
    public static string GetGuid()
    {
        return Guid.NewGuid().ToString();
    }
}