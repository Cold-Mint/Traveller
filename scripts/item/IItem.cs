using Godot;

namespace ColdMint.scripts.item;

public interface IItem
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

    /// <summary>
    /// <para>Execute when current item is used <br/> e.g. when player clicks left mouse button with current item in hand</para>
    /// </summary>
    /// <param name="owner">Owner of current item, if any</param>
    /// <param name="targetGlobalPosition">Target position, such as the position of the cursor when used by the player</param>
    void Use(Node2D? owner, Vector2 targetGlobalPosition);

    /// <summary>
    /// <para>Execute when current item be removed from game.</para>
    /// </summary>
    void Destroy();
}