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

//var query1 = dbContext.Products
//    .Take(10)
//    .Select(x => new ProductListDto
//    {
//        Id = x.Id,
//        Name = x.Translations.First(t => t.Code == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value
//    });

//var result1 = await query1.ToListAsync();

//var query2 = dbContext.ProductsJson
//    .Take(10)
//    .Select(x => new ProductListDto
//    {
//        Id = x.Id,
//        Name = x.Translations.First(t => t.Code == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value
//    });
//var result2 = await query2.ToListAsync();

//var query22 = dbContext.ProductsJson
//    .Take(10)
//    .ProjectTo<ProductListDto>(mapperCfg);

//var result22 = await query22.ToListAsync();

var query3 = dbContext.ProductsJson2
    .AsNoTracking()
    .Take(10)
    .Select(x => new ProductListDto
    {
        Id = x.Id,
        Name = x.Name.Translations.First(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value
    });
var result3 = await query3.ToListAsync();

var query33 = dbContext.ProductsJson2
    .AsNoTracking()
    .Take(10)
    .ProjectTo<ProductListDto>(mapperCfg);
var result33 = await query33.ToListAsync();



var query4 = dbContext.ProductsJson2
    .AsNoTracking()
    .Take(10)
    .Select(x => new ProductDto
    {
        Id = x.Id,
        Name = x.Name.Translations.ToList()
    });
var result4 = await query4.ToListAsync();

var query44 = dbContext.ProductsJson2
    .AsNoTracking()
    .Take(10)
    .ProjectTo<ProductDto>(mapperCfg);
var result44 = await query44.ToListAsync();


var query5 = dbContext.ProductsJson2
    .AsNoTracking()
    .Take(10);
var result5 = await query5.ToListAsync();

var x1 = mapper.Map<List<ProductDto>>(result5);
var x2 = mapper.Map<List<ProductListDto>>(result5);

Console.WriteLine();

static Action<IMapperConfigurationExpression> GetConfigAction(params Assembly[] assemblies)
{
    return cfg =>
    {
        //cfg.Internal().ForAllMaps((tm, me) => me.IgnoreAllPropertiesWithAnInaccessibleSetter());
        //cfg.ShouldMapField = x => x.IsPublic;
        //cfg.ShouldUseConstructor = x => x.IsPublic;
        //cfg.ShouldMapMethod = x => false;

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
        //AllowNullCollections = true;
        //AllowNullDestinationValues = true;

        //CreateMap<LocalizedText, string?>()
        //    .ConvertUsing((x, y) =>
        //    {
        //        return x.Translations.FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)?.Value;
        //    });

        CreateMap<LocalizedText, string?>()
            .ConvertUsing(x => x.Translations.FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName));

        //CreateMap<TranslationJson2, string?>()
        //    .ConvertUsing(x => x.Value);

        //CreateMap<LocalizedText, string?>()
        //    .ConvertUsing(x => x.Translations.FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName) == null 
        //                    ? null 
        //                    : x.Translations.First(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value);


        CreateMap<LocalizedText, List<TranslationJson2>>()
            .ConvertUsing(x => x.Translations.ToList());


        CreateMap<ProductJson2, ProductListDto>();
        CreateMap<ProductJson2, ProductDto>();




        CreateMap<Product, ProductListDto>()
            .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Translations.First(t => t.Code == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value));

        CreateMap<ProductJson, ProductListDto>()
            .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Translations.First(t => t.Code == CultureInfo.CurrentCulture.TwoLetterISOLanguageName).Value));
    }
}
