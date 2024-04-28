using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ColdMint.scripts.database;
using ColdMint.scripts.database.dataPackEntity;
using ColdMint.scripts.debug;

namespace ColdMint.scripts.dataPack.entryLoader;

public class SpriteLoader : IEntryLoader
{
    private HashSet<string> _spriteNameSet = new HashSet<string>();

    public bool NeedLoad(ZipArchiveEntry archiveEntry)
    {
        return archiveEntry.FullName.StartsWith(Config.SpriteStartPathName);
    }

    public async Task ExecutionLoad(string namespaceString, string zipFileName, DataPackDbContext dataPackDbContext,
        ZipArchiveEntry archiveEntry)
    {
        var fileName = Path.GetFileNameWithoutExtension(archiveEntry.FullName);
        if (_spriteNameSet.Contains(fileName))
        {
            LogCat.LogErrorWithFormat("duplicate_at_path_id", zipFileName, archiveEntry.FullName, fileName);
            return;
        }

        var spriteDbSet = dataPackDbContext.SpriteInfo;
        //如果没有记录，创建一份。
        var spriteInfo = new SpriteInfo
        {
            FileName = fileName,
            Namespace = namespaceString,
            FullName = archiveEntry.FullName,
            ZipFileName = zipFileName,
            CrateTime = DateTime.Now
        };
        await spriteDbSet.AddAsync(spriteInfo);
        _spriteNameSet.Add(fileName);
    }
}