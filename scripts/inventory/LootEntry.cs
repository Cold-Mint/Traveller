namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>Loot entry</para>
/// <para>战利品条目</para>
/// </summary>
public class LootEntry
{
    /// <summary>
    /// <para>generation probability</para>
    /// <para>生成概率</para>
    /// </summary>
    public double? Chance { get; set; }

    /// <summary>
    /// <para>Minimum number of generated</para>
    /// <para>最小生成多少个</para>
    /// </summary>
    public int MinQuantity { get; set; }
    
    /// <summary>
    /// <para>The maximum number of files to be generated</para>
    /// <para>最多生成多少个</para>
    /// </summary>
    public int MaxQuantity { get; set; }

    /// <summary>
    /// <para>resources path</para>
    /// <para>资源路径</para>
    /// </summary>
    public string? ResPath { get; set; }
}