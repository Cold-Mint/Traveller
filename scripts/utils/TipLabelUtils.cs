using Godot;

namespace ColdMint.scripts.utils;

public static class TipLabelUtils
{
    /// <summary>
    /// <para>ShowTip</para>
    /// <para>显示提示标签</para>
    /// </summary>
    /// <param name="rotation">
    ///<para>The rotation value of the node</para>
    ///<para>节点的旋转值</para>
    /// </param>
    /// <param name="tipLabel">
    ///<para>tipLabel</para>
    ///<para>要显示的标签节点</para>
    /// </param>
    /// <param name="tip">
    ///<para>tip</para>
    ///<para>显示的提示内容</para>
    /// </param>
    public static void ShowTip(float rotation, Label tipLabel, string tip)
    {
        tipLabel.Visible = true;
        tipLabel.Text = tip;
        tipLabel.ResetSize();
        //Vertical Centering Tip
        //垂直居中提示
        var newPosition = tipLabel.Position;
        newPosition.X = -tipLabel.Size.X / 2;
        tipLabel.Rotation = rotation;
        tipLabel.Position = newPosition;
    }

    /// <summary>
    /// <para>HideTip</para>
    /// <para>隐藏提示标签</para>
    /// </summary>
    /// <param name="tipLabel"></param>
    public static void HideTip(Label tipLabel)
    {
        tipLabel.Visible = false;
    }
}