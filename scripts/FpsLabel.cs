using Godot;

namespace ColdMint.scripts;

/// <summary>
/// <para>FPSLabel</para>
/// <para>FPS标签</para>
/// </summary>
public partial class FpsLabel : Label
{
    bool _enable;
    private LabelSettings? _labelSettings;

    public override void _Ready()
    {
        Text = null;
        if (Config.IsDebug())
        {
            _labelSettings = new LabelSettings();
            LabelSettings = _labelSettings;
            _enable = true;
        }
    }

    public override void _Process(double delta)
    {
        if (!_enable)
        {
            return;
        }

        var fps = Engine.GetFramesPerSecond();
        Text = "FPS:" + fps;
        if (_labelSettings != null)
        {
            //Green above 54 frames (smooth)
            //在54帧以上为绿色（流畅）
            if (fps > 54)
            {
                _labelSettings.FontColor = Colors.Green;
            }
            else if (fps > 48)
            {
                //Yellow between 48 and 54 frames (Karting)
                //在48到54帧之间为黄色（卡顿）
                _labelSettings.FontColor = Colors.Yellow;
            }
            else
            {
                //Red below 48 frames (lag)
                //在48帧以下为红色（卡）
                _labelSettings.FontColor = Colors.Red;
            }
        }
    }
}