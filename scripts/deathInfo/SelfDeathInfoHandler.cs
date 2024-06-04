using System.Threading.Tasks;
using ColdMint.scripts.character;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.deathInfo;

/// <summary>
/// <para>Deal with the message that your game failed due to accidental injury</para>
/// <para>处理自己误伤导致游戏失败的信息</para>
/// </summary>
public class SelfDeathInfoHandler : IDeathInfoHandler
{
    private const string Prefix = "death_info_self_";
    private const int Length = 2;

    public Task<string?> GenerateDeathInfo(string victimName, string killerName, Player victim, Node killer)
    {
        if (victim != killer) return Task.FromResult<string?>(null);
        var index = GD.Randi() % Length + 1;
        return Task.FromResult(
            TranslationServerUtils.TranslateWithFormat(Prefix + index, victimName, killerName));

    }
}