using AutoMapper;
using AutoMapper.Internal;
using AutoMapper.QueryableExtensions;
using BenchmarkDotNet.Running;
using EFJsonBenchmarks;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;

//BenchmarkRunner.Run<Benchmark>();

CultureInfo.CurrentCulture = new CultureInfo("fr");
var mapperCfg = new MapperConfiguration(GetConfigAction(typeof(Program).Assembly));
var mapper = new Mapper(mapperCfg);


using var dbContext = new AppDbContext();

var query1 = dbContext.Products
    .Take(10)
    .Select(x => new ProductDto
    {
        Id = x.Id,
        Name = x.Translations.First(t => t.Code == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value
    });

var result1 = await query1.ToListAsync();

var query2 = dbContext.ProductsJson
    .Take(10)
    .Select(x => new ProductDto
    {
        Id = x.Id,
        Name = x.Translations.First(t => t.Code == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value
    });
var result2 = await query2.ToListAsync();

var query22 = dbContext.ProductsJson
    .Take(10)
    .ProjectTo<ProductDto>(mapperCfg);

var result22 = await query22.ToListAsync();

var query3 = dbContext.ProductsJson2
    .Take(10)
    .Select(x => new ProductDto
    {
        Id = x.Id,
        Name = x.Name.Translations.First(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value
    });
var result3 = await query3.ToListAsync();

var query33 = dbContext.ProductsJson2
    .AsNoTracking()
    .Take(10)
    .ProjectTo<ProductDto>(mapperCfg);
var result33 = await query33.ToListAsync();



Console.WriteLine();

static Action<IMapperConfigurationExpression> GetConfigAction(params Assembly[] assemblies)
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

public class ProductMapper : Profile
{
    public ProductMapper()
    {
        CreateMap<LocalizedText, string?>()
            .ConvertUsing(x => x.Translations.First(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value);

            //.ForMember(x => x, opt => opt.MapFrom(x => x.Translations.First(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value));

        CreateMap<Product, ProductDto>()
            .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Translations.First(t => t.Code == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value));

        CreateMap<ProductJson, ProductDto>()
            .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Translations.First(t => t.Code == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value));

        CreateMap<ProductJson2, ProductDto>();
           //.IncludeMembers(x => x.Name);
           //.ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name.Translations.First(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value));
    }
}
