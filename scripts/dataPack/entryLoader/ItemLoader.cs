using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ColdMint.scripts.database;
using ColdMint.scripts.database.dataPackEntity;
using ColdMint.scripts.debug;
using ColdMint.scripts.serialization;

namespace ColdMint.scripts.dataPack.entryLoader;

/// <summary>
/// <para>Load item information into the data table in the manifest file</para>
/// <para>在清单文件内加载物品信息到数据表</para>
/// </summary>
public class ItemLoader : IEntryLoader
{
    private HashSet<string> _itemIdSet = new HashSet<string>();

    public bool NeedLoad(ZipArchiveEntry archiveEntry)
    {
        return archiveEntry.FullName.StartsWith(Config.ItemStartPathName) &&
               archiveEntry.FullName.EndsWith(Config.DataPackSourceFileFomat);
    }

    public async Task ExecutionLoad(string namespaceString, string zipFileName, DataPackDbContext dataPackDbContext,
        ZipArchiveEntry archiveEntry)
    {
        await using var stream = archiveEntry.Open();
        //从文件中读取物品信息
        var itemInfo = await JsonSerialization.ReadJsonFileToObj<ItemInfo>(stream);
        if (itemInfo == null)
        {
            return;
        }

        if (_itemIdSet.Contains(itemInfo.Id))
        {
            LogCat.LogErrorWithFormat("duplicate_at_path_id", zipFileName, archiveEntry.FullName, itemInfo.Id);
            return;
        }

        if (itemInfo.MaxStackQuantity <= 0 || itemInfo.MaxStackQuantity > Config.MaxStackQuantity)
        {
            itemInfo.MaxStackQuantity = Config.MaxStackQuantity;
        }

        if (itemInfo.Quantity <= 0)
        {
            itemInfo.Quantity = 1;
        }

        if (itemInfo.Quantity > Config.MaxStackQuantity)
        {
            itemInfo.Quantity = Config.MaxStackQuantity;
        }

        itemInfo.Namespace = namespaceString;
        var itemDbSet = dataPackDbContext.ItemInfo;
        itemInfo.ZipFileName = zipFileName;
        itemInfo.CrateTime = DateTime.Now;
        await itemDbSet.AddAsync(itemInfo);
        _itemIdSet.Add(itemInfo.Id);
    }
}