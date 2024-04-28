using System.Collections.Generic;

namespace ColdMint.scripts.inventory;

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
        var key = GetKey(item);
        if (_dictionary.ContainsKey(key))
        {
            return;
        }

        _dictionary.Add(key, item);
    }

    private static string GetKey(IItem item)
    {
        return item.Namespace + item.Id;
    }
}