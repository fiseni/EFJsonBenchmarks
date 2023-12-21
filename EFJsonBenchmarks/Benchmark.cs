using AutoMapper;
using AutoMapper.Internal;
using AutoMapper.QueryableExtensions;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;

namespace EFJsonBenchmarks;

[MemoryDiagnoser]
public class Benchmark
{
    private AppDbContext _dbContext1 = default!;
    private AppDbContext _dbContext2 = default!;
    private AppDbContext _dbContext3 = default!;
    private string _currentCultureCode = default!;
    private MapperConfiguration _mapperConfiguration = default!;

    [GlobalSetup]
    public async Task Setup()
    {
        // It will create the DB and seed data, 1M records for Product and ProductJson. It may take some time.
        // If you seed the data separately, keep this line. This will initialize the EF model, so we don't have that overhead in the benchmarks.
        await DbInitializer.SeedAsync();

        // Let's just keep separate contexts to be fair.
        _dbContext1 = new AppDbContext();
        _dbContext2 = new AppDbContext();
        _dbContext3 = new AppDbContext();

        _currentCultureCode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        _mapperConfiguration = new MapperConfiguration(GetConfigAction(typeof(Program).Assembly));
        Console.WriteLine("### Setup completed!");
    }

    private static Action<IMapperConfigurationExpression> GetConfigAction(params Assembly[] assemblies)
    {
        return cfg =>
        {
            cfg.Internal().ForAllMaps((tm, me) => me.IgnoreAllPropertiesWithAnInaccessibleSetter());
            cfg.ShouldMapField = x => x.IsPublic;
            cfg.ShouldUseConstructor = x => x.IsPublic;
            cfg.ShouldMapMethod = x => false;

            if (assemblies.Any())
            {
                cfg.AddMaps(assemblies);
            }
        };
    }

    [Params(10, 100, 1000)]
    public int Take { get; set; }

    [Benchmark(Baseline = true)]
    public async Task WithSeparateTable()
    {
        _ = await _dbContext1.Products
            .AsNoTracking()
            .Take(Take)
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Translations.First(t => t.Code == _currentCultureCode).Value
            })
            .ToListAsync();
    }

    [Benchmark]
    public async Task WithJsonColumn()
    {
        _ = await _dbContext2.ProductsJson
            .AsNoTracking()
            .Take(Take)
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Translations.First(t => t.Code == _currentCultureCode).Value
            })
            .ToListAsync();
    }

    [Benchmark]
    public async Task WithJsonColumn2()
    {
        _ = await _dbContext3.ProductsJson2
            .AsNoTracking()
            .Take(Take)
            .Select(x => new ProductListDto
            {
                Id = x.Id,
                Name = x.Name.Translations.FirstOrDefault(t => t.LanguageCode == _currentCultureCode)
            })
            .ToListAsync();
    }

    [Benchmark]
    public async Task WithJsonColumn22()
    {
        _ = await _dbContext3.ProductsJson2
            .AsNoTracking()
            .Take(Take)
            .ProjectTo<ProductListDto>(_mapperConfiguration)
            .ToListAsync();
    }
}
