using System.Text;
using ColdMint.scripts.character;
using ColdMint.scripts.furniture;
using ColdMint.scripts.inventory;
using ColdMint.scripts.loot;
using Godot;

namespace ColdMint.scripts.utils;

public static class FloatLabelUtils
{
    /// <summary>
    /// <para>ShowTip</para>
    /// <para>显示提示标签</para>
    /// </summary>
    /// <param name="obj">
    ///<para>The object information displayed in debug mode.</para>
    ///<para>在debug模式下显示的对象信息。</para>
    /// </param>
    /// <param name="tip">
    ///<para>tip</para>
    ///<para>显示的提示内容</para>
    /// </param>
    /// <param name="fontColor">
    ///<para>fontColor</para>
    ///<para>字体颜色</para>
    /// </param>
    public static void ShowFloatLabel(object obj, string tip, Color fontColor)
    {
        var floatLabel = GameSceneDepend.FloatLabel;
        if (floatLabel == null)
        {
            return;
        }


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
                stringBuilder.Append("\nCharacterName:");
                stringBuilder.Append(character.ReadOnlyCharacterName);
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

            if (obj is Furniture furniture)
            {
                stringBuilder.Append("\nDurability:");
                stringBuilder.Append(furniture.Durability);
                stringBuilder.Append("\nMaxDurability:");
                stringBuilder.Append(furniture.MaxDurability);
                stringBuilder.Append("\nLootId:");
                stringBuilder.Append(furniture.LootId);
            }

            floatLabel.Text = stringBuilder.ToString();
        }
        else
        {
            floatLabel.Text = tip;
        }

        var labelSettings = floatLabel.LabelSettings;
        if (labelSettings == null)
        {
            floatLabel.LabelSettings = new LabelSettings
            {
                FontColor = fontColor
            };
        }
        else
        {
            labelSettings.FontColor = fontColor;
        }

        floatLabel.Follow = true;
        floatLabel.Show();
    }

    /// <summary>
    /// <para>HideFloatLabel</para>
    /// <para>隐藏悬浮标签</para>
    /// </summary>
    public static void HideFloatLabel()
    {
        GameSceneDepend.FloatLabel?.Hide();
    }
}