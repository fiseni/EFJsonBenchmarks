using AutoMapper;
using EFJsonBenchmarks;
using System.Globalization;


public class ProductMapper : Profile
{
    public ProductMapper()
    {
        CreateMap<LocalizedText, string?>()
            .ConvertUsing(x => x.Translations.FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName));

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
