namespace ColdMint.scripts.map.interfaces;

/// <summary>
/// <para>Represents a branch on the map.</para>
/// <para>表示地图上的一个分支。</para>
/// </summary>
public interface IBranch
{
    /// <summary>
    /// <para>Master branch or not</para>
    /// <para>是否为主分支</para>
    /// </summary>
    bool IsMasterBranch
    {
        get;
        set;
    }
}