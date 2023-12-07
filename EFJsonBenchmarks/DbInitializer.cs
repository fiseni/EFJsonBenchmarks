using Microsoft.EntityFrameworkCore;

namespace EFJsonBenchmarks;

public static class DbInitializer
{
    public static async Task SeedAsync()
    {
        using var dbContext = new AppDbContext();

        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Products.AnyAsync() == false)
        {
            var products = Enumerable.Range(1, 1_000_000).Select(x => new Product 
            { 
                Translations = 
                [
                    new Translation(0, "en", "English"),
                    new Translation(0, "de", "German")
                ] 
            });
            dbContext.Products.AddRange(products);
        }

        if (await dbContext.ProductsJson.AnyAsync() == false)
        {
            var productsJson = Enumerable.Range(1, 1_000_000).Select(x => new ProductJson
            {
                Translations =
                [
                    new TranslationJson("en", "English"),
                    new TranslationJson("de", "German")
                ]
            });
            dbContext.ProductsJson.AddRange(productsJson);
        }

        await dbContext.SaveChangesAsync();
    }
}
