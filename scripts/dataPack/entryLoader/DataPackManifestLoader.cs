using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ColdMint.scripts.database;
using ColdMint.scripts.database.dataPackEntity;
using ColdMint.scripts.dataPack.local;
using ColdMint.scripts.serialization;

namespace ColdMint.scripts.dataPack.entryLoader;

/// <summary>
/// <para>Load the manifest file in the zip package and write it to the data table</para>
/// <para>在zip包内加载清单文件，将其写入数据表</para>
/// </summary>
public class DataPackManifestLoader : IEntryLoader
{
    public string Namespace => _namespace;

    //清单文件的命名空间
    private string _namespace;

    public bool NeedLoad(ZipArchiveEntry archiveEntry)
    {
        return archiveEntry.FullName == Config.DataPackManifestName;
    }

    public async Task ExecutionLoad(string namespaceString, string zipFileName, DataPackDbContext dataPackDbContext,
        ZipArchiveEntry archiveEntry)
    {
        //Do not use namespaceString within the DataPackManifestLoader's ExecutionLoad method, as this value is assigned in the following code.
        //不要在DataPackManifestLoader的ExecutionLoad方法内使用namespaceString，因为这个值是在下面的代码内赋值的。
        var nowDateTime = DateTime.Now;
        IDataPackManifest? dataPackManifest = null;
        //When the manifest file is obtained, load the file information
        //在获取到清单文件时，加载文件信息
        await using (var stream = archiveEntry.Open())
        {
            var localDataPackManifest =
                await JsonSerialization.ReadJsonFileToObj<LocalDataPackManifest>(stream);
            if (localDataPackManifest == null)
            {
                dataPackManifest = EmptyManifest.CreateEmptyManifest(zipFileName);
            }
            else
            {
                dataPackManifest = localDataPackManifest;
            }
        }

        if (dataPackManifest != null)
        {
            var dataPackInfoDbSet = dataPackDbContext.DataPackInfo;
            var dataPackQuery = from dataPack in dataPackInfoDbSet
                where dataPack.ZipFileName == zipFileName
                select dataPack;
            var oldDataPackInfo = dataPackQuery.FirstOrDefault();
            if (oldDataPackInfo == null)
            {
                //There was no list to record before, create one.
                //之前没有清单记录，创建一份。
                await dataPackDbContext.DataPackInfo.AddAsync(new DataPackInfo
                {
                    ID = dataPackManifest.ID,
                    Author = dataPackManifest.Author,
                    Description = dataPackManifest.Description,
                    Name = dataPackManifest.Name,
                    Namespace = dataPackManifest.Namespace,
                    VersionCode = dataPackManifest.VersionCode,
                    VersionName = dataPackManifest.VersionName,
                    ZipFileName = zipFileName,
                    UpdateTime = nowDateTime,
                    CrateTime = nowDateTime
                });
            }
            else
            {
                //It's already on the record. Update.
                //已经有记录了，更新。
                oldDataPackInfo.Name = dataPackManifest.Name;
                oldDataPackInfo.Author = dataPackManifest.Author;
                oldDataPackInfo.Description = dataPackManifest.Description;
                oldDataPackInfo.Namespace = dataPackManifest.Namespace;
                oldDataPackInfo.VersionCode = dataPackManifest.VersionCode;
                oldDataPackInfo.VersionName = dataPackManifest.VersionName;
                oldDataPackInfo.UpdateTime = nowDateTime;
                dataPackDbContext.DataPackInfo.Update(oldDataPackInfo);
            }

            _namespace = dataPackManifest.Namespace ?? string.Empty;
        }
    }
}