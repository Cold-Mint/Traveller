using System;
using System.ComponentModel.DataAnnotations;
using ColdMint.scripts.dataPack;

namespace ColdMint.scripts.database.dataPackEntity;

public class DataPackInfo : IDataPackManifest
{
    [Key] public string? ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? VersionName { get; set; }
    public int? VersionCode { get; set; }
    public string? Author { get; set; }
    public string? Namespace { get; set; }

    /// <summary>
    /// <para>Whether the status is enabled</para>
    /// <para>是否为启用状态</para>
    /// </summary>
    public bool IsEnabled { get; set; }
    
    public string ZipFileName { get; set; }
    
    public DateTime CrateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}