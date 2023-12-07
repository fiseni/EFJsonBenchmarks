using Microsoft.EntityFrameworkCore;

namespace EFJsonBenchmarks;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductJson> ProductsJson => Set<ProductJson>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EFJsonBenchmarks;Integrated Security=SSPI;Trusted_Connection=True";
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasMany(x => x.Translations);
        modelBuilder.Entity<Translation>().HasIndex(x => x.Code); // We'll add index to be fair.

        modelBuilder.Entity<ProductJson>().OwnsMany(x=>x.Translations, builder => builder.ToJson());
    }
}
