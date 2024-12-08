using System.Threading.Tasks;
using ColdMint.scripts.character;
using Godot;

namespace ColdMint.scripts.deathInfo;

/// <summary>
/// <para>Death information processor</para>
/// <para>死亡信息处理器</para>
/// </summary>
public interface IDeathInfoHandler
{
    /// <summary>
    /// <para>Generate death info</para>
    /// <para>生成死亡信息</para>
    /// </summary>
    /// <param name="victimName">
    ///<para>victimName</para>
    ///<para>受害者名称</para>
    /// </param>
    /// <param name="killerName">
    ///<para>KillerName</para>
    ///<para>杀手名称</para>
    /// </param>
    /// <param name="victim">
    /// <para>victim</para>
    /// <para>受害者</para>
    /// </param>
    /// <param name="killer">
    /// <para>Killer</para>
    /// <para>杀手</para>
    /// </param>
    /// <returns></returns>
    public Task<string?> GenerateDeathInfoAsync(string victimName, string killerName, Player victim, Node killer);
}