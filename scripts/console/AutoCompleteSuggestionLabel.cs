using Godot;

namespace ColdMint.scripts.console;

/// <summary>
/// <para>Auto Complete Suggestion Label</para>
/// <para>自动完成建议标签</para>
/// </summary>
public partial class AutoCompleteSuggestionLabel : RichTextLabel
{
    public LineEdit? LineEdit;

    public AutoCompleteSuggestion AutoCompleteSuggestion;

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
        if (LineEdit == null)
        {
            return;
        }

        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true })
        {
            var inputText = LineEdit.Text;
            if (string.IsNullOrEmpty(inputText))
            {
                SetLineEditText(AutoCompleteSuggestion.Value);
                return;
            }

            var index = inputText.LastIndexOf(' ');
            if (index == -1)
            {
                SetLineEditText(AutoCompleteSuggestion.Value);
                return;
            }

            if (index == inputText.Length - 1)
            {
                SetLineEditText(inputText + AutoCompleteSuggestion.Value);
                return;
            }

            SetLineEditText(inputText[..index] +" "+ AutoCompleteSuggestion.Value);
        }
    }

    private void SetLineEditText(string text)
    {
        if (LineEdit == null)
        {
            return;
        }

        LineEdit.Text = text + " ";
        LineEdit.EmitSignal("text_changed", text);
        LineEdit.CaretColumn = text.Length + 1;
    }
}