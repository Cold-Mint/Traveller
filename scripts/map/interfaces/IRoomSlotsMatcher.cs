using System.Threading.Tasks;
using ColdMint.scripts.map.dateBean;

namespace ColdMint.scripts.map.interfaces;

/// <summary>
/// <para>IRoomSlotsMatcher</para>
/// <para>房间插槽匹配器</para>
/// </summary>
public interface IRoomSlotsMatcher
{
    /// <summary>
    /// <para>Whether the slots of the two rooms can be matched</para>
    /// <para>两个房间的插槽是否可匹配</para>
    /// </summary>
    /// <param name="mainRoom"></param>
    /// <param name="newRoom"></param>
    /// <returns></returns>
    Task<bool> IsMatch(IRoom? mainRoom, IRoom newRoom);

   
    /// <summary>
    /// <para>LastMatchedMainSlot</para>
    /// <para>最后匹配的主要插槽</para>
    /// </summary>
    RoomSlot? LastMatchedMainSlot { get; }
    
    /// <summary>
    /// <para>LastMatchedMinorSlot</para>
    /// <para>最后匹配的次要插槽</para>
    /// </summary>
    RoomSlot? LastMatchedMinorSlot { get; }
}