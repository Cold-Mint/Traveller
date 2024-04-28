using System;
using System.Linq;
using System.Threading.Tasks;
using ColdMint.scripts.database;
using ColdMint.scripts.database.dataPackEntity;
using ColdMint.scripts.debug;
using ColdMint.scripts.serialization;
using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>Local Item</para>
/// <para>本地Item</para>
/// </summary>
public class LocalItem : IItem
{
    private ItemInfo _itemInfo;
    private int quantity;

    public LocalItem(ItemInfo itemInfo)
    {
        _itemInfo = itemInfo;
        quantity = itemInfo.Quantity;
    }

    public string Id => _itemInfo.Id;

    public int Quantity
    {
        get => quantity;
        set { quantity = value; }
    }

    public int MaxStackQuantity => _itemInfo.MaxStackQuantity;
    public Texture2D Icon { get; set; }
    public string Name => _itemInfo.Name;
    public string Namespace => _itemInfo.Namespace;
    public Action<IItem> OnUse { get; set; }
    public Func<IItem, Node> OnInstantiation { get; set; }
}