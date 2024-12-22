using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColdMint.scripts.debug;
using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts.console;

/// <summary>
/// <para>ConsoleGui</para>
/// <para>控制台的Gui页面</para>
/// </summary>
public partial class ConsoleGui : Control
{
    [Export] private Button? _submitButton;

    [Export] private LineEdit? _commandEdit;

    [Export] private VBoxContainer? _logContainer;

    [Export] private VBoxContainer? _suggestedContainer;

    public static ConsoleGui? Instance { get; private set; }

    private DateTime _checkTextChange;
    private readonly TimeSpan _measureInterval = TimeSpan.FromMilliseconds(50);

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        if (_submitButton != null)
        {
            _submitButton.Pressed += Pressed;
        }

        if (_commandEdit != null)
        {
            _commandEdit.TextChanged += TextChanged;
        }
    }

    private void TextChanged(string text)
    {
        _checkTextChange = DateTime.Now + _measureInterval;
    }

    /// <summary>
    /// <para>When the button is clicked</para>
    /// <para>当按钮被点击时</para>
    /// </summary>
    private void Pressed()
    {
        Submit();
    }

    /// <summary>
    /// <para>Echo</para>
    /// <para>在控制台上输出信息</para>
    /// </summary>
    /// <param name="message"></param>
    public void Echo(string message)
    {
        if (_logContainer == null)
        {
            return;
        }

        var label = new Label();
        label.Text = message;
        NodeUtils.CallDeferredAddChild(_logContainer, label);
        LogCat.Log(message);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("console") && Config.IsDebug())
        {
            Visible = !Visible;
        }

        if (_commandEdit != null && DateTime.Now >= _checkTextChange)
        {
            _checkTextChange = DateTime.MaxValue;
            var text = _commandEdit.Text;
            var charArray = text.ToCharArray();
            var level = 0;
            var lastLevelIndex = 0;
            var firstLevelIndex = 0;
            for (var i = 0; i < charArray.Length; i++)
            {
                var c = charArray[i];
                if (c != ' ') continue;
                level++;
                lastLevelIndex = i;
                if (firstLevelIndex == 0)
                {
                    firstLevelIndex = i;
                }
            }


            var commandName = firstLevelIndex > 0 ? text[..firstLevelIndex] : text;
            if (level == 0)
            {
                ShowSuggestList(CommandManager.GetSuggest(commandName));
                return;
            }

            var command = CommandManager.GetCommand(commandName);
            if (command == null)
            {
                return;
            }

            var maxLevel = command.Suggest.Length;
            if (level > maxLevel)
            {
                LogCat.LogWithFormat("too_many_level_requested", LogCat.LogLabel.Default, level, maxLevel);
                return;
            }

            var keyWord = lastLevelIndex == text.Length - 1 ? null : text[(lastLevelIndex + 1)..];
            ShowSuggestList(
                SuggestUtils.ScreeningSuggestion(command.Suggest[level - 1], keyWord));
        }
    }

    /// <summary>
    /// <para>ShowSuggestList</para>
    /// <para>显示建议列表</para>
    /// </summary>
    /// <param name="suggestionList"></param>
    private void ShowSuggestList(IEnumerable<string> suggestionList)
    {
        if (_suggestedContainer == null)
        {
            return;
        }

        NodeUtils.DeleteAllChild(_suggestedContainer);
        foreach (var suggest in suggestionList)
        {
            var richTextLabel = new RichTextLabel();
            richTextLabel.BbcodeEnabled = true;
            richTextLabel.FitContent = true;
            richTextLabel.Text = suggest;
            NodeUtils.CallDeferredAddChild(_suggestedContainer, richTextLabel);
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventKey { Keycode: Key.Enter, Pressed: true })
        {
            Pressed();
        }
    }

    private async Task Submit()
    {
        if (_commandEdit == null || _submitButton == null)
        {
            return;
        }

        var code = _commandEdit.Text;
        if (string.IsNullOrEmpty(code))
        {
            return;
        }

        _commandEdit.Text = "";
        _commandEdit.Editable = false;
        _submitButton.Disabled = true;
        await CommandExecutor.ExecuteCommandAsync(code);
        _submitButton.Disabled = false;
        _commandEdit.Editable = true;
    }
}