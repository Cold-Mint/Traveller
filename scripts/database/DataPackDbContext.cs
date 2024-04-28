using ColdMint.scripts.database.dataPackEntity;
using ColdMint.scripts.debug;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ColdMint.scripts.database;

public class DataPackDbContext : DbContext
{
    public DbSet<DataPackInfo> DataPackInfo { get; set; }

    public DbSet<ZipEntryInfo> ZipEntryInfo { get; set; }
    public DbSet<ZipFileInfo> ZipFileInfo { get; set; }

    public DbSet<ItemInfo> ItemInfo { get; set; }
    public DbSet<SpriteInfo> SpriteInfo { get; set; }

    public DataPackDbContext(DbContextOptions<DataPackDbContext> options) : base(options)
    {
    }
}