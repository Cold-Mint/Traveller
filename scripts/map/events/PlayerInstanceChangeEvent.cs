using ColdMint.scripts.character;

namespace ColdMint.scripts.map.events;

/// <summary>
/// <para>Player instance change event</para>
/// <para>玩家实例改变事件</para>
/// </summary>
public class PlayerInstanceChangeEvent
{
    /// <summary>
    /// <para>New player instance</para>
    /// <para>新的玩家实例</para>
    /// </summary>
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public Player? PlayerInstance { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global
}