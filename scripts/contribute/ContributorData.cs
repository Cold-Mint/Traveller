namespace ColdMint.scripts.contribute;

/// <summary>
/// <para>Contributor information</para>
/// <para>贡献者数据</para>
/// </summary>
public class ContributorData
{
    /// <summary>
    /// <para>Contributor's name</para>
    /// <para>贡献者的名字</para>
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// <para>Links to contributors' home pages</para>
    /// <para>贡献者的主页链接</para>
    /// </summary>
    public string? Url { get; set; }
    
    /// <summary>
    /// <para>Type of contribution</para>
    /// <para>贡献的类型</para>
    /// </summary>
    public ContributorType[]? ContributorTypes { get; set; }
}