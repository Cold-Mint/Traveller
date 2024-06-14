namespace ColdMint.scripts.contribute;

/// <summary>
/// <para>Contribution type</para>
/// <para>贡献类型</para>
/// </summary>
public enum ContributorType
{
    /// <summary>
    /// <para>Coder</para>
    /// <para>程序员</para>
    /// </summary>
    /// <remarks>
    ///<para>Contributed code to the project</para>
    ///<para>为项目贡献了代码</para>
    /// </remarks>
    Coder,
    /// <summary>
    /// <para>Artist</para>
    /// <para>美术</para>
    /// </summary>
    /// <remarks>
    ///<para>Contributed art assets to the project</para>
    ///<para>为项目贡献了美术资产</para>
    /// </remarks>
    Artist,
    /// <summary>
    /// <para>Musician</para>
    /// <para>音乐家</para>
    /// </summary>
    /// <remarks>
    ///<para>Contributed music, sound assets to the project</para>
    ///<para>为项目贡献了音乐，音效资产</para>
    /// </remarks>
    Musician,
    /// <summary>
    /// <para>CharacterVoice</para>
    /// <para>角色配音</para>
    /// </summary>
    /// <remarks>
    ///<para>Contributed voice to the game character</para>
    ///<para>为游戏角色贡献了语音</para>
    /// </remarks>
    CharacterVoice,
    /// <summary>
    /// <para>Translator</para>
    /// <para>翻译者</para>
    /// </summary>
    /// <remarks>
    ///<para>Contribute to localization of the project(specified language)</para>
    ///<para>为项目的本地化做出贡献(指定的语言)</para>
    /// </remarks>
    Translator
}