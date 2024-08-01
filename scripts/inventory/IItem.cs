using Godot;

namespace ColdMint.scripts.inventory;

public interface IItem
{
    /// <summary>
    /// <para>ID of current item</para>
    /// <para>当前物品的ID</para>
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// <para>Icon of current item</para>
    /// <para>当前项目的图标</para>
    /// </summary>
    Texture2D Icon { get; }

    /// <summary>
    /// <para>Display name of current item</para>
    /// <para>显示当前Item的名称</para>
    /// </summary>
    string Name { get; }

    /// <summary>
    /// <para>Description of current item, which may show in inventory</para>
    /// <para>当前项目的描述</para>
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// <para>Quantity</para>
    /// <para>当前的数量</para>
    /// </summary>
    int Quantity { get; set; }
    
    /// <summary>
    /// <para>MaxItemQuantity</para>
    /// <para>最大物品数量</para>
    /// </summary>
    int MaxQuantity { get; }

    /// <summary>
    /// <para>Execute when current item is used <br/> e.g. when player clicks left mouse button with current item in hand</para>
    /// <para>当前项被使用时执行 <br/> e.g. 当玩家用鼠标左键点击当前物品时</para>
    /// </summary>
    /// <param name="owner">Owner of current item, if any</param>
    /// <param name="targetGlobalPosition">Target position, such as the position of the cursor when used by the player</param>
    void Use(Node2D? owner, Vector2 targetGlobalPosition);
}