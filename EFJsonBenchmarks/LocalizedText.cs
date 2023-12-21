using Ardalis.GuardClauses;
using System.Globalization;

namespace EFJsonBenchmarks;

public class LocalizedText
{
    public string? Value
    {
        get
        {
            var currentLanguageCode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return Translations.FirstOrDefault(x => x.LanguageCode != null && x.LanguageCode.Equals(currentLanguageCode))?.Value;
        }
    }

    private readonly List<TranslationJson2> _translations = new();
    public IEnumerable<TranslationJson2> Translations => _translations.AsEnumerable();

    private LocalizedText() { }

    public LocalizedText(IEnumerable<TranslationJson2> translations)
    {
        AddOrUpdate(translations);
    }

    public void AddOrUpdate(IEnumerable<TranslationJson2> translations)
    {
        foreach (var translation in translations)
        {
            AddOrUpdate(translation);
        }
    }

    public void AddOrUpdate(TranslationJson2 translation)
    {
        Guard.Against.Null(translation);

        var existingTranslation = Translations.FirstOrDefault(x => x.LanguageCode == translation.LanguageCode);

        if (existingTranslation is not null)
        {
            existingTranslation.LanguageCode = translation.LanguageCode;
            existingTranslation.Value = translation.Value;
        }
        else
        {
            _translations.Add(translation);
        }
    }
}
