using Godot;

namespace ColdMint.scripts.item;

//Todo: Merge this with IItem (and, then change this ugly name
public interface IItem_New
{
    /// <summary>
    /// <para>ID of current item</para>
    /// </summary>
    string Id { get; }
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