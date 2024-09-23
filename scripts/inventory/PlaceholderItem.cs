using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>A blank item used to occupy a space in the display. The IsSelect property is set to true so that the item display always appears selected when drawn.</para>
/// <para>物品容器显示器内用于占位的空白物品。IsSelect属性设置为true，使得物品显示器绘制时总是显示为选中状态。</para>
/// </summary>
public class PlaceholderItem : IItem
{
    public string Id { get; set; }
    public Texture2D Icon { get; }
    public string Name { get; }
    public string? Description { get; } = null;
    public int Quantity { get; set; } = 1;
    public int MaxQuantity { get; } = 1;
    public bool IsSelect { get; set; } = true;

    public int MergeableItemCount(IItem other, int unallocatedQuantity)
    {
        return 0;
    }

    public IItem? CreateItem(int number)
    {
        return null;
    }

    public void Use(Node2D? owner, Vector2 targetGlobalPosition)
    {
    }
}