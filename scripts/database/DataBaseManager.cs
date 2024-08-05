using System;
using System.IO;

namespace ColdMint.scripts.database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <para>Game database manager</para>
/// <para>游戏数据库管理器</para>
/// </summary>
public class DataBaseManager
{
    private static ServiceProvider? _serviceProvider;

    /// <summary>
    /// <para>Initialize database</para>
    /// <para>初始化数据库</para>
    /// </summary>
    public static void InitDataBases(string databasePath)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<GameDbContext>(options =>
            options.UseSqlite($"Data Source={Path.Join(databasePath, Config.SolutionName + ".db")}"));
        _serviceProvider = serviceCollection.BuildServiceProvider();
        var dataPackDbContext = GetRequiredService<GameDbContext>();
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
        if (_serviceProvider == null)
        {
            throw new NullReferenceException("service provider is null");
        }

        var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }
}