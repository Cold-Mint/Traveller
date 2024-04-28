namespace ColdMint.scripts.dataPack;

/// <summary>
/// <para>DataPackManifest</para>
/// <para>数据包清单文件</para>
/// </summary>
public interface IDataPackManifest
{
    string? ID { get; set; }
    string? Name { get; set; }
    string? Description { get; set; }
    string? VersionName { get; set; }
    int? VersionCode { get; set; }
    string? Author { get; set; }
    string? Namespace { get; set; }
}