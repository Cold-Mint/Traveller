using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColdMint.scripts.database.dataPackEntity;

public class SpriteInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Index { get; set; }

    /// <summary>
    /// <para>The file name is the Id of the Sprite</para>
    /// <para>文件名就是精灵的Id</para>
    /// </summary>
    public string FileName { get; set; }
    
    public string FullName { get; set; }
    
    public string ZipFileName { get; set; }
    
    public string Namespace { get; set; }

    public DateTime CrateTime { get; set; }
}