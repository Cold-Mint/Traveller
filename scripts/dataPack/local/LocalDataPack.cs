using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ColdMint.scripts.database;
using ColdMint.scripts.database.dataPackEntity;
using ColdMint.scripts.dataPack.entryLoader;
using ColdMint.scripts.debug;
using ColdMint.scripts.serialization;
using ColdMint.scripts.utils;
using Godot;
using Microsoft.EntityFrameworkCore;

namespace ColdMint.scripts.dataPack.local;

/// <summary>
/// <para>LocalDataPack</para>
/// <para>本地数据包</para>
/// </summary>
public class LocalDataPack : IDataPack
{
    private string zipFilePath;
    private string zipFileName;
    private IDataPackManifest? manifest;

    public LocalDataPack(string zipFilePath)
    {
        this.zipFilePath = zipFilePath;
        zipFileName = Path.GetFileName(zipFilePath);
    }


    /// <summary>
    /// <para>Create index information for packets in the database</para>
    /// <para>在数据库内为数据包创建索引信息</para>
    /// </summary>
    public async Task BuildIndex()
    {
        //我们首先根据文件名在数据表内查找对应的Md5值，在判断Md5值是否发生变化。
        var entryLoaders = new List<IEntryLoader>();
        entryLoaders.Add(new ItemLoader());
        entryLoaders.Add(new SpriteLoader());
        var md5 = Md5Utils.GetFileMd5(zipFilePath);
        var dataPackDbContext = DataBaseManager.GetRequiredService<DataPackDbContext>();
        var zipFileInfoDbSet = dataPackDbContext.ZipFileInfo;
        var query = from zipFileInfo in zipFileInfoDbSet
            where zipFileInfo.ZipFileName == zipFileName
            select zipFileInfo;
        var oldZipFileInfo = await query.FirstOrDefaultAsync();
        if (oldZipFileInfo == null || oldZipFileInfo.ZipFileMd5 != md5)
        {
            //Get the list file GetEntry internal Dictionary based implementation, very fast
            //获取清单文件 GetEntry内部基于Dictionary实现，速度很快
            //If there is no manifest file, we do not scan the zip and only save the Md5 value for next check
            //如果没有清单文件，我们不扫描zip，仅保存Md5值，以便下次检查
            using var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Read, Encoding.GetEncoding("GBK"));
            var manifestEntry = archive.GetEntry(Config.DataPackManifestName);
            if (manifestEntry == null)
            {
                LogCat.LogErrorWithFormat("no_manifest_file", zipFilePath);
            }
            else
            {
                var dataPackManifestLoader = new DataPackManifestLoader();
                await dataPackManifestLoader.ExecutionLoad(string.Empty, zipFileName, dataPackDbContext, manifestEntry);
                LogCat.LogWithFormat("build_an_index", zipFilePath);
                var zipEntryInfoDbSet = dataPackDbContext.ZipEntryInfo;
                if (oldZipFileInfo != null)
                {
                    //Delete old records that are outdated
                    //删除过时的旧记录
                    var entriesToDelete = zipEntryInfoDbSet
                        .Where(entry => entry.FileName == zipFileName)
                        .ToList();
                    dataPackDbContext.ZipEntryInfo.RemoveRange(entriesToDelete);

                    var itemsToDelete = dataPackDbContext.ItemInfo
                        .Where(item => item.ZipFileName == zipFileName)
                        .ToList();
                    dataPackDbContext.ItemInfo.RemoveRange(itemsToDelete);

                    var spritesToDelete = dataPackDbContext.SpriteInfo
                        .Where(sprite => sprite.ZipFileName == zipFileName)
                        .ToList();
                    dataPackDbContext.SpriteInfo.RemoveRange(spritesToDelete);
                    await dataPackDbContext.SaveChangesAsync();
                }

                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(Config.ZipPathCharacter))
                    {
                        //Ignore folders
                        //忽略文件夹
                        continue;
                    }

                    var nowDateTime = DateTime.Now;
                    foreach (var entryLoader in entryLoaders)
                    {
                        var needLoad = entryLoader.NeedLoad(entry);
                        if (needLoad)
                        {
                            await entryLoader.ExecutionLoad(dataPackManifestLoader.Namespace, zipFileName,
                                dataPackDbContext, entry);
                        }
                    }

                    var zipEntryInfo = new ZipEntryInfo
                    {
                        FullName = entry.FullName,
                        FileName = zipFileName,
                        CrateTime = nowDateTime
                    };
                    LogCat.LogWithFormat("add_file_index", entry.FullName);
                    await zipEntryInfoDbSet.AddAsync(zipEntryInfo);
                }
            }

            if (oldZipFileInfo == null)
            {
                //创建一份
                var zipNowDateTime = DateTime.Now;
                var zipFileInfo = new ZipFileInfo
                {
                    ZipFileName = zipFileName,
                    ZipFileMd5 = md5,
                    EntryCount = archive.Entries.Count,
                    CrateTime = zipNowDateTime,
                    UpdateTime = zipNowDateTime
                };
                dataPackDbContext.ZipFileInfo.Add(zipFileInfo);
            }
            else
            {
                var zipNowDateTime = DateTime.Now;
                oldZipFileInfo.UpdateTime = zipNowDateTime;
                oldZipFileInfo.ZipFileMd5 = md5;
                oldZipFileInfo.EntryCount = archive.Entries.Count;
                dataPackDbContext.Update(oldZipFileInfo);
            }

            try
            {
                await dataPackDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                LogCat.LogError(e);
            }

            LogCat.LogWithFormat("index_updated", zipFilePath);
            return;
        }

        //没有变化什么也不做
        LogCat.LogWithFormat("index_is_up_to_date", zipFilePath);
    }

    /// <summary>
    /// <para>Load manifest file</para>
    /// <para>加载清单文件</para>
    /// </summary>
    public async Task LoadManifest()
    {
        var dataPackDbContext = DataBaseManager.GetRequiredService<DataPackDbContext>();
        var dataPackInfoDbSet = dataPackDbContext.DataPackInfo;
        var dataPackInfo = from dataPack in dataPackInfoDbSet
            where dataPack.ZipFileName == zipFileName
            select dataPack;
        if (dataPackInfo != null)
        {
            manifest = await dataPackInfo.FirstOrDefaultAsync();
        }
    }


    public IDataPackManifest Manifest => manifest ?? EmptyManifest.CreateEmptyManifest(zipFileName);

    /// <summary>
    /// <para>Get the item's data</para>
    /// <para>获取物品的数据</para>
    /// </summary>
    /// <returns></returns>
    public string GetItemsData()
    {
        return Path.Join(zipFilePath, "items");
    }
}