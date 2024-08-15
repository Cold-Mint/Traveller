using ColdMint.scripts.database.gameDbTables;
using Microsoft.EntityFrameworkCore;

namespace ColdMint.scripts.database;

/// <summary>
/// <para>Game database</para>
/// <para>游戏数据库</para>
/// </summary>
public class GameDbContext(DbContextOptions<GameDbContext> options) : DbContext(options)
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public DbSet<ErrorRecord> ErrorRecords { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global

    /// <summary>
    /// <para>Async add error record</para>
    /// <para>异步添加错误信息</para>
    /// </summary>
    /// <param name="errorRecord"></param>
    public async void AddOrUpdateErrorRecordAsync(ErrorRecord errorRecord)
    {
        if (errorRecord.Message == null)
        {
            return;
        }

        var oldErrorRecord = await ErrorRecords.FindAsync(errorRecord.Message);
        if (oldErrorRecord == null)
        {
            ErrorRecords.Add(errorRecord);
        }
        else
        {
            oldErrorRecord.Count++;
            oldErrorRecord.LastDateTime = errorRecord.LastDateTime;
            ErrorRecords.Update(oldErrorRecord);
        }

        await SaveChangesAsync();
    }
}