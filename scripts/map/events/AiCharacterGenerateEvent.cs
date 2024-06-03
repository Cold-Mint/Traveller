namespace ColdMint.scripts.map.events;

/// <summary>
/// <para>AiCharacterGenerateEvent</para>
/// <para>Ai角色生成事件</para>
/// </summary>
public class AiCharacterGenerateEvent
{
    /// <summary>
    /// <para>Map generation completed Tag</para>
    /// <para>地图生成完成的Tag</para>
    /// </summary>
    public const string TagMapGenerationComplete = "MapGenerationComplete";
    

    /// <summary>
    /// <para>The Tag used to generate the role</para>
    /// <para>生成角色时使用的Tag</para>
    /// </summary>
    public string? Tag { get; set; }
}