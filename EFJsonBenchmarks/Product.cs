namespace EFJsonBenchmarks;

public class Product
{
    public int Id { get; set; }
    public List<Translation> Translations { get; set; } = new();
}

public record Translation(int Id, string Code, string Value);