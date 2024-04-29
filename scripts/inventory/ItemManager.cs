using System.Collections.Generic;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>Item manager</para>
/// <para>物品管理器</para>
/// </summary>
public static class ItemManager
{
    private static Dictionary<string, IItem> _dictionary = new Dictionary<string, IItem>();

    /// <summary>
    /// <para>Add items to Item Manager</para>
    /// <para>在物品管理器内添加物品</para>
    /// </summary>
    /// <param name="item"></param>
    public static void AddItem(IItem item)
    {
        if (_dictionary.ContainsKey(item.Id))
        {
            return;
        }
        _dictionary.Add(item.Id, item);
    }
}