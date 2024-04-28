using System.IO.Compression;
using System.Threading.Tasks;
using ColdMint.scripts.database;

namespace ColdMint.scripts.dataPack.entryLoader;

public interface IEntryLoader
{
    /// <summary>
    /// <para>Whether to load</para>
    /// <para>是否需要加载</para>
    /// </summary>
    /// <param name="archiveEntry"></param>
    /// <returns></returns>
    bool NeedLoad(ZipArchiveEntry archiveEntry);


    /// <summary>
    /// <para>Execution load</para>
    /// <para>执行加载</para>
    /// </summary>
    ///<remarks>
    ///<para>It is only necessary to add or update data to dataPackDbContext in this method. When the scan is completed, the upper layer code will be uniformly submitted to the database</para>
    ///<para>仅需要在此方法内将数据add或者update到dataPackDbContext内，当扫描结束后，上层代码会统一提交到数据库</para>
    /// <para>Do not query the existence of the old project from the database within this method, because the save request is also submitted to the database.</para>
    /// <para>不要在此方法内从数据库查询旧的项目是否存在，因为还为向数据库提交保存请求。</para>
    /// </remarks>
    /// <param name="archiveEntry"></param>
    Task ExecutionLoad(string namespaceString, string zipFileName, DataPackDbContext dataPackDbContext,
        ZipArchiveEntry archiveEntry);
}