using System.Collections.Generic;

namespace ColdMint.scripts.camp;

/// <summary>
/// <para>camp</para>
/// <para>阵营</para>
/// </summary>
public class Camp
{
    private string _id;
    private List<string> _friendlyCampIdList;

    public Camp(string id)
    {
        _id = id;
        _friendlyCampIdList = new List<string>();
    }

    /// <summary>
    /// <para>Get camp ID</para>
    /// <para>获取阵营ID</para>
    /// </summary>
    public string ID => _id;

    /// <summary>
    /// <para>Get camp name</para>
    /// <para>获取阵营名</para>
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// <para>Friend Injury</para>
    /// <para>友伤</para>
    /// </summary>
    /// <remarks>
    ///<para>Whether to damage targets on the same side</para>
    ///<para>是否可伤害同一阵营的目标</para>
    /// </remarks>
    public bool FriendInjury { get; set; }

    /// <summary>
    /// <para>Gets the camp ID that is friendly to this camp</para>
    /// <para>获取与此阵营友好的阵营ID</para>
    /// </summary>
    public string[] FriendlyCampIdArray => _friendlyCampIdList.ToArray();
}