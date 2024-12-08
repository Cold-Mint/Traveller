using System.Collections.Generic;
using System.Threading.Tasks;
using ColdMint.scripts.character;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.deathInfo;

public static class DeathInfoGenerator
{
    private static List<IDeathInfoHandler>? _deathInfoHandlers;

    /// <summary>
    /// <para>Register the death message handler</para>
    /// <para>注册死亡信息处理器</para>
    /// </summary>
    /// <param name="deathInfoHandler"></param>
    public static void RegisterDeathInfoHandler(IDeathInfoHandler deathInfoHandler)
    {
        _deathInfoHandlers ??= new List<IDeathInfoHandler>();
        _deathInfoHandlers.Add(deathInfoHandler);
    }

    /// <summary>
    /// <para>Unregister the death message handler</para>
    /// <para>取消注册死亡信息处理器</para>
    /// </summary>
    /// <param name="deathInfoHandler"></param>
    public static void UnregisterDeathInfoHandler(IDeathInfoHandler deathInfoHandler)
    {
        if (_deathInfoHandlers == null)
        {
            return;
        }

        _deathInfoHandlers.Remove(deathInfoHandler);
    }

    /// <summary>
    /// <para>Generate death info</para>
    /// <para>生成死亡信息</para>
    /// </summary>
    /// <param name="victim"></param>
    /// <param name="killer"></param>
    /// <returns></returns>
    public static async Task<string> GenerateDeathInfoAsync(Player victim, Node killer)
    {
        var victimName = victim.ReadOnlyCharacterName ?? victim.Name;
        string killerName = killer.Name;
        if (killer is CharacterTemplate characterTemplate)
        {
            killerName = characterTemplate.ReadOnlyCharacterName ?? killer.Name;
        }
        if (_deathInfoHandlers == null || _deathInfoHandlers.Count == 0)
        {
            return GenerateDefaultDeathInfo(victimName, killerName) ?? string.Empty;
        }
        foreach (var deathInfoHandler in _deathInfoHandlers)
        {
            var deathInfo = await deathInfoHandler.GenerateDeathInfoAsync(victimName, killerName, victim, killer);
            if (!string.IsNullOrEmpty(deathInfo))
            {
                return deathInfo;
            }
        }

        return GenerateDefaultDeathInfo(victimName, killerName) ?? string.Empty;
    }

    /// <summary>
    /// <para>Generate a default death message</para>
    /// <para>生成默认的死亡信息</para>
    /// </summary>
    /// <param name="victimName"></param>
    /// <param name="killerName"></param>
    /// <returns></returns>
    private static string? GenerateDefaultDeathInfo(string victimName, string killerName)
    {
        return TranslationServerUtils.TranslateWithFormat("death_info_default", victimName, killerName);
    }
}