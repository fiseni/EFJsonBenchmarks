namespace EFJsonBenchmarks;

public class ProductJson
{
    public int Id { get; set; }
    public List<TranslationJson> Translations { get; set; } = new();
}

public record TranslationJson(string Code, string Value);
