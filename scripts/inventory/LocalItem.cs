using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColdMint.scripts.database;
using ColdMint.scripts.database.dataPackEntity;
using ColdMint.scripts.debug;
using ColdMint.scripts.serialization;
using Godot;
using Microsoft.EntityFrameworkCore;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>Local Item</para>
/// <para>本地Item</para>
/// </summary>
public class LocalItem : IItem
{
    private ItemInfo _itemInfo;
    private int quantity;
    private ImageTexture texture2D;

    public LocalItem(ItemInfo itemInfo)
    {
        _itemInfo = itemInfo;
        quantity = itemInfo.Quantity;
    }

    public async Task LoadIcon()
    {
        if (_itemInfo == null)
        {
            return;
        }

        var icon = _itemInfo.Icon;
        if (icon == null)
        {
            return;
        }

        //查找图标路径
        var dataPackDbContext = DataBaseManager.GetRequiredService<DataPackDbContext>();
        var spriteDbSet = dataPackDbContext.SpriteInfo;
        //在同一个命名空间下查找
        var query = from sprite in spriteDbSet
            where sprite.FileName == icon && sprite.Namespace == _itemInfo.Namespace
            select sprite;
        var spriteInfo = await query.FirstOrDefaultAsync();
        if (spriteInfo == null)
        {
            return;
        }

        var zipFilePath = Path.Join(Config.GetDataPackDirectory(), spriteInfo.ZipFileName);
        using var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Read, Encoding.GetEncoding("GBK"));
        var zipArchiveEntry = archive.GetEntry(spriteInfo.FullName);
        if (zipArchiveEntry == null)
        {
            return;
        }

        var outPath = Path.Join(Config.GetDataPackCacheDirectory(_itemInfo.Namespace), spriteInfo.FileName+".jpg");
        zipArchiveEntry.ExtractToFile(outPath);
        var image = Image.LoadFromFile(outPath);
        if (image == null)
        {
            LogCat.Log("无法加载"+outPath);
            return;
        }
        texture2D = ImageTexture.CreateFromImage(image);
    }

    public string Id => _itemInfo.Id;

    public int Quantity
    {
        get => quantity;
        set { quantity = value; }
    }

    public int MaxStackQuantity => _itemInfo.MaxStackQuantity;
    public Texture2D Icon => texture2D;
    public string Name => _itemInfo.Name;
    public string Namespace => _itemInfo.Namespace;
    public Action<IItem> OnUse { get; set; }
    public Func<IItem, Node> OnInstantiation { get; set; }
}