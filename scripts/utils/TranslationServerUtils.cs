using Godot;

namespace ColdMint.scripts.utils;

/// <summary>
/// <para>Translation server utils</para>
/// <para>翻译服务器工具</para>
/// </summary>
public static class TranslationServerUtils
{
    /// <summary>
    /// <para>Gets a translation of a field and displays it formatted</para>
    /// <para>获取某个字段的翻译，并且将其格式化显示</para>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string? TranslateWithFormat(string key, params object[] args)
    {
        var value = TranslationServer.Translate(key);
        return value == null ? null : string.Format(value, args);
    }

    /// <summary>
    /// <para>Gets a translation of a field</para>
    /// <para>获取某个字段的翻译</para>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string? Translate(string? key)
    {
        return key == null ? null : TranslationServer.Translate(key);
    }
}