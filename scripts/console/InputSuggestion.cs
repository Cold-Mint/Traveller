namespace ColdMint.scripts.console;

/// <summary>
/// <para>Suggest</para>
/// <para>输入建议</para>
/// </summary>
/// <param name="DisplayText">
///<para>DisplayText</para>
///<para>显示的文本</para>
/// </param>
/// <param name="Value">
///<para>Value</para>
///<para>实际值</para>
/// </param>
public record struct InputSuggestion(string DisplayText, string Value);