using ColdMint.scripts.debug;
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

    public override void _Ready()
    {
        base._Ready();
        if (_submitButton != null)
        {
            _submitButton.Pressed += Submit;
        }
    }

    /// <summary>
    /// <para>Echo</para>
    /// <para>在控制台上输出信息</para>
    /// </summary>
    /// <param name="message"></param>
    public static void Echo(string message)
    {
        LogCat.Log(message);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("console") && Config.IsDebug())
        {
            Visible = !Visible;
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventKey { Keycode: Key.Enter, Pressed: true })
        {
            Submit();
        }
    }

    private async void Submit()
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