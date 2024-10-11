using System.Collections.Generic;
using ColdMint.scripts.debug;

namespace ColdMint.scripts.camp;

/// <summary>
/// <para>Camp manager</para>
/// <para>阵营管理器</para>
/// </summary>
public static class CampManager
{
    private static readonly Dictionary<string, Camp?> Camps = new();

    /// <summary>
    /// <para>The default camp is returned if no corresponding camp is obtained</para>
    /// <para>当获取不到对应的阵营时，返回的默认阵营</para>
    /// </summary>
    private static Camp? _defaultCamp;

    /// <summary>
    /// <para>SetDefaultCamp</para>
    /// <para>设置默认阵营</para>
    /// </summary>
    /// <param name="camp">
    ///<para>Camp, whose ID must be the default camp ID.</para>
    ///<para>阵营，要求其ID必须为默认阵营ID。</para>
    /// </param>
    /// <returns>
    ///<para>Return whether the setting is successful</para>
    ///<para>返回是否设置成功</para>
    /// </returns>
    public static bool SetDefaultCamp(Camp? camp)
    {
        if (camp == null)
        {
            LogCat.Log("camp_is_null", label: LogCat.LogLabel.CampManager);
            return false;
        }

        if (camp.Id != Config.CampId.Default) return false;
        _defaultCamp = camp;
        AddCamp(camp);
        LogCat.LogWithFormat("set_default_camp", label: LogCat.LogLabel.CampManager,  camp.Id);
        return true;
    }

    /// <summary>
    /// <para>Whether camp A can damage camp B</para>
    /// <para>阵营A是否可伤害阵营B</para>
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool CanCauseHarm(Camp? attacker, Camp? target)
    {
        if (attacker == null || target == null)
        {
            LogCat.Log("attacker_or_target_is_null", label: LogCat.LogLabel.CampManager);
            return false;
        }

        if (attacker.Id == target.Id)
        {
            //In the same camp, return whether friendly fire is allowed
            //在同一阵营内，返回是否允许友伤
            LogCat.Log("in_the_same_camp", label: LogCat.LogLabel.CampManager);
            return attacker.FriendInjury;
        }

        //A camp ID that the attacker considers friendly
        //攻击者认为友好的阵营ID
        var friendlyCampIdArray = attacker.FriendlyCampIdArray;
        var targetId = target.Id;
        if (friendlyCampIdArray.Length > 0)
        {
            foreach (var friendlyCampId in friendlyCampIdArray)
            {
                if (friendlyCampId == targetId)
                {
                    //The attacker thinks the target is friendly, and we can't hurt a friendly target
                    //攻击者认为目标友好，我们不能伤害友好的目标
                    LogCat.Log("friendly_target", label: LogCat.LogLabel.CampManager);
                    return false;
                }
            }
        }

        LogCat.Log("can_cause_harm", label: LogCat.LogLabel.CampManager);
        return true;
    }


    /// <summary>
    /// <para>Add camp</para>
    /// <para>添加阵营</para>
    /// </summary>
    /// <param name="camp"></param>
    /// <returns></returns>
    public static bool AddCamp(Camp? camp)
    {
        return camp != null && Camps.TryAdd(camp.Id, camp);
    }

    /// <summary>
    /// <para>Get camp based on ID</para>
    /// <para>根据ID获取阵营</para>
    /// </summary>
    /// <remarks>
    ///<para>Cannot get back to default camp</para>
    ///<para>获取不到返回默认阵营</para>
    /// </remarks>
    /// <param name="id">
    ///<para>Camp ID</para>
    ///<para>阵营ID</para>
    /// </param>
    /// <returns></returns>
    public static Camp? GetCamp(string? id)
    {
        if (id == null)
        {
            return _defaultCamp;
        }

        return Camps.GetValueOrDefault(id, _defaultCamp);
    }
}