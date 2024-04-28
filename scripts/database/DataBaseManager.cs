using System.IO;
using ColdMint.scripts.database.dataPackEntity;
using ColdMint.scripts.debug;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ColdMint.scripts.database;

/// <summary>
/// <para>Game database manager</para>
/// <para>游戏数据库管理器</para>
/// </summary>
public static class DataBaseManager
{
    private static ServiceProvider serviceProvider;

    /// <summary>
    /// <para>Initialize database</para>
    /// <para>初始化数据库</para>
    /// </summary>
    public static void InitDataBases(string databasePath)
    {
        if (!Directory.Exists(databasePath))
        {
            return;
        }

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<DataPackDbContext>(options =>
            options.UseSqlite($"Data Source={Path.Join(databasePath, "DataPack.db")}"));
        serviceProvider = serviceCollection.BuildServiceProvider();
        var dataPackDbContext = GetRequiredService<DataPackDbContext>();
        dataPackDbContext.Database.EnsureCreated();
    }

    
    /// <summary>
    /// <para>Get database service</para>
    /// <para>获取数据库服务</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetRequiredService<T>() where T : notnull
    {
        var scope = serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }
}