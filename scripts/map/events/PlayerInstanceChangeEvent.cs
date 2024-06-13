using ColdMint.scripts.character;

namespace ColdMint.scripts.map.events;

/// <summary>
/// <para>Player instance change event</para>
/// <para>玩家实例改变事件</para>
/// </summary>
public class PlayerInstanceChangeEvent
{
    public Player? PlayerInstance { get; set; }
}