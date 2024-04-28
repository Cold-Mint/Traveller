using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColdMint.scripts.database.dataPackEntity;

/// <summary>
/// <para>entry table in Zip file</para>
/// <para>Zip文件内的entry表</para>
/// </summary>
public class ZipEntryInfo
{
    /// <summary>
    /// <para>Primary key, auto increment</para>
    /// <para>主键，自动递增</para>
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    
    /// <summary>
    /// <para>This record is from that zip file</para>
    /// <para>这段记录是来源于那个zip文件的</para>
    /// </summary>
    public string FileName { get; set; }
    
    /// <summary>
    /// <para>The path within the zip file</para>
    /// <para>位于zip文件内的路径</para>
    /// </summary>
    public string FullName { get; set; }
    
    public DateTime CrateTime { get; set; }

}