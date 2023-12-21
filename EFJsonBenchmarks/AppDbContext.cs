using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFJsonBenchmarks;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductJson> ProductsJson => Set<ProductJson>();
    public DbSet<ProductJson2> ProductsJson2 => Set<ProductJson2>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EFJsonBenchmarks;Integrated Security=SSPI;Trusted_Connection=True";
        optionsBuilder.UseSqlServer(connectionString).EnableSensitiveDataLogging().LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasMany(x => x.Translations);
        modelBuilder.Entity<Translation>().HasIndex(x => x.Code); // We'll add index to be fair.

        modelBuilder.Entity<ProductJson>().OwnsMany(x=>x.Translations, builder => builder.ToJson());

        modelBuilder.Entity<ProductJson2>().OwnsOne(x => x.Name, x => x.ToJson().OwnsMany(x => x.Translations));

    }
}
