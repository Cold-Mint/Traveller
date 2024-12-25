namespace ColdMint.scripts.console;

/// <summary>
/// <para>Command Args</para>
/// <para>命令参数</para>
/// </summary>
public readonly record struct CommandArgs(string[] StrArray)
{
    public int Length => StrArray.Length;

    /// <summary>
    /// <para>Gets the arguments at a location</para>
    /// <para>获取某个位置的参数</para>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string? GetString(int index) => IsIndexValid(index) ? StrArray[index] : null;

    /// <summary>
    /// <para>Take the argument at a position and convert it to an int</para>
    /// <para>获取某个位置的参数，并将其转为int</para>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public int GetInt(int index, int defaultValue = 0) =>
        IsIndexValid(index) && int.TryParse(StrArray[index], out var result) ? result : defaultValue;

    /// <summary>
    /// <para>Take a positional argument and convert it to a float</para>
    /// <para>获取某个位置的参数，并将其转为float</para>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public float GetFloat(int index, float defaultValue = 0) =>
        IsIndexValid(index) && float.TryParse(StrArray[index], out var result) ? result : defaultValue;

    /// <summary>
    /// <para>Take the argument at a position and convert it to a double</para>
    /// <para>获取某个位置的参数，并将其转为double</para>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public double GetDouble(int index, double defaultValue = 0) =>
        IsIndexValid(index) && double.TryParse(StrArray[index], out var result) ? result : defaultValue;

    /// <summary>
    /// <para>Takes a positional argument and converts it to a long</para>
    /// <para>获取某个位置的参数，并将其转换为long</para>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public long GetLong(int index, long defaultValue = 0) =>
        IsIndexValid(index) && long.TryParse(StrArray[index], out var result) ? result : defaultValue;

    /// <summary>
    /// <para>Take a positional argument and convert it to a Bool</para>
    /// <para>获取某个位置的参数，并将其转为Bool</para>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public bool GetBool(int index, bool defaultValue = false) =>
        IsIndexValid(index) && bool.TryParse(StrArray[index], out var result) ? result : defaultValue;

    /// <summary>
    /// <para>Whether it is a valid index</para>
    /// <para>是否为合法的索引</para>
    /// </summary>
    /// <param name="index">
    ///<para>index</para>
    ///<para>索引</para>
    /// </param>
    /// <returns></returns>
    private bool IsIndexValid(int index) => index >= 0 && index < StrArray.Length;
}