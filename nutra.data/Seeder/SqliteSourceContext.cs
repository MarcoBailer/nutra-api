using Microsoft.EntityFrameworkCore;
using Nutra.Seeder.Models;

namespace Nutra.Seeder;

public class SqliteSourceContext : DbContext
{
    private readonly string _dbPath;

    public SqliteSourceContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_dbPath}");

    public DbSet<TbcaSqlite> Tbcas { get; set; }
    public DbSet<FastFoodSqlite> FastFoods { get; set; }
    public DbSet<FabricantesSqlite> Fabricantes { get; set; }
    public DbSet<GenericosSqlite> Genericos { get; set; }
}
