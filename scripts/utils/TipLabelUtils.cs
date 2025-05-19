using System.Text;
using ColdMint.scripts.character;
using ColdMint.scripts.furniture;
using ColdMint.scripts.inventory;
using ColdMint.scripts.serialization;
using Godot;
using YamlDotNet.Serialization.NodeDeserializers;

namespace ColdMint.scripts.utils;

public static class TipLabelUtils
{
    /// <summary>
    /// <para>ShowTip</para>
    /// <para>显示提示标签</para>
    /// </summary>
    /// <param name="obj">
    ///<para>The object information displayed in debug mode.</para>
    ///<para>在debug模式下显示的对象信息。</para>
    /// </param>
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
    /// <param name="fontColor">
    ///<para>fontColor</para>
    ///<para>字体颜色</para>
    /// </param>
    public static void ShowTip(object obj, float rotation, Label tipLabel, string tip, Color fontColor)
    {
        tipLabel.Visible = true;
        if (GameSceneDepend.ShowObjectDetails)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(tip);
            stringBuilder.Append('\n');
            stringBuilder.Append(obj.GetType());
            stringBuilder.Append("\nHashCode:");
            stringBuilder.Append(obj.GetHashCode());
            if (obj is Node2D node2D)
            {
                stringBuilder.Append("\nPosition:");
                stringBuilder.Append(node2D.Position);
                stringBuilder.Append("\nGlobalPosition:");
                stringBuilder.Append(node2D.GlobalPosition);
            }

            if (obj is CharacterTemplate character)
            {
                stringBuilder.Append("\nCharacterName");
                stringBuilder.Append(character.CharacterName);
                stringBuilder.Append("\nCampId:");
                stringBuilder.Append(character.CampId);
                stringBuilder.Append("\nHp:");
                stringBuilder.Append(character.Hp);
                stringBuilder.Append("\nMapHp:");
                stringBuilder.Append(character.ReadOnlyMaxHp);
                stringBuilder.Append("\nItemContainer:");
                var itemContainer = character.ItemContainer;
                if (itemContainer == null)
                {
                    stringBuilder.Append("null");
                }
                else
                {
                    stringBuilder.Append("\nTotalCapacity:");
                    stringBuilder.Append(itemContainer.GetTotalCapacity());
                    stringBuilder.Append("\nUsedCapacity:");
                    stringBuilder.Append(itemContainer.GetUsedCapacity());
                }
            }

            if (obj is IItem item)
            {
                stringBuilder.Append("\nItemId:");
                stringBuilder.Append(item.Id);
                stringBuilder.Append("\nItemType:");
                stringBuilder.Append(item.ItemType);
                stringBuilder.Append("\nQuantity:");
                stringBuilder.Append(item.Quantity);
                stringBuilder.Append("\nMaxQuantity:");
                stringBuilder.Append(item.MaxQuantity);
            }

            tipLabel.Text = stringBuilder.ToString();
        }
        else
        {
            tipLabel.Text = tip;
        }

        var labelSettings = tipLabel.LabelSettings;
        if (labelSettings == null)
        {
            tipLabel.LabelSettings = new LabelSettings
            {
                FontColor = fontColor
            };
        }
        else
        {
            labelSettings.FontColor = fontColor;
        }

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