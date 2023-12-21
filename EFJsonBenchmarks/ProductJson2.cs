using Ardalis.GuardClauses;

namespace EFJsonBenchmarks;

public class ProductJson2
{
    public int Id { get; set; }
    public required LocalizedText Name { get; set; }
}

public class TranslationJson2
{
    private string _languageCode = default!;

    public required string LanguageCode
    {
        get => _languageCode;
        set => _languageCode = Guard.Against.NullOrEmpty(value);
    }

    public required string? Value { get; set; }

    public static implicit operator string?(TranslationJson2? translation) => translation?.Value ?? default;
}
