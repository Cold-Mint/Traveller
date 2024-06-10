using Godot;

namespace ColdMint.scripts.item;

public interface IItemStack
{
    /// <summary>
    /// <para>ID of items inside current stack</para>
    /// </summary>
    string Id { get; }

    /// <summary>
    /// <para>Max number of current stack</para>
    /// </summary>
    int MaxQuantity { get; }

    /// <summary>
    /// <para>Quantity of current stack</para>
    /// </summary>
    int Quantity { get; set; }
    
    /// <summary>
    /// <para>Icon of current item</para>
    /// </summary>
    Texture2D Icon { get; }
    
    /// <summary>
    /// <para>Display name of current item</para>
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// <para>Description of current item, which may show in inventory</para>
    /// </summary>
    string? Description { get; }
}