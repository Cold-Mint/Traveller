using System.Threading.Tasks;
using ColdMint.scripts.character;
using ColdMint.scripts.inventory;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.deathInfo;

public class ResignationCertificateDeathInfoHandler: IDeathInfoHandler
{
    public Task<string?> GenerateDeathInfo(string victimName, string killerName, Player victim, Node killer)
    {
        if (killer is ResignationCertificate)
        {
            return Task.FromResult(
                TranslationServerUtils.TranslateWithFormat("death_info_resignation_certificate", victimName));
        }
        return Task.FromResult(Config.EmptyString);
    }
}