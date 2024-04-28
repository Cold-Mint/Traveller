using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ColdMint.scripts.serialization;

namespace ColdMint.scripts.dataPack.local;

public class LocalDataPackManifest: IDataPackManifest
{
    public string? ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? VersionName { get; set; }
    public int? VersionCode { get; set; }
    public string? Author { get; set; }
    public string? Namespace { get; set; }
}