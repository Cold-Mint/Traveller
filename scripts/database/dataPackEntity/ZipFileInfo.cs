using System;
using System.ComponentModel.DataAnnotations;

namespace ColdMint.scripts.database.dataPackEntity;

public class ZipFileInfo
{
    [Key] public string ZipFileName { get; set; }

    public string ZipFileMd5 { get; set; }

    public int EntryCount { get; set; }
    public DateTime CrateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}