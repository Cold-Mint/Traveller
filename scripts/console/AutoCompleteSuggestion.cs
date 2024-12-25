namespace ColdMint.scripts.console;

/// <summary>
/// <para>Auto complete Suggestion</para>
/// <para>自动完成建议</para>
/// </summary>
/// <param name="DisplayText">
///<para>DisplayText</para>
///<para>显示的文本</para>
/// </param>
/// <param name="Value">
///<para>Value</para>
///<para>实际值</para>
/// </param>
public record struct AutoCompleteSuggestion(string DisplayText, string Value);