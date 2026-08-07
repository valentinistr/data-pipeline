namespace Consumer.Models;

public class IngestionPackage<TData, TRow>(IReadOnlyCollection<TData> validData, IReadOnlyCollection<TRow> invalidData)
{
    public int InvalidRows => InvalidData.Count;
    public int ValidRows => ValidData.Count;
    
    public IReadOnlyCollection<TData>  ValidData { get; } = validData;
    public IReadOnlyCollection<TRow> InvalidData { get; } = invalidData;
    
    public static IngestionPackage<TData, TRow> Empty => new IngestionPackage<TData, TRow>([], []);
}