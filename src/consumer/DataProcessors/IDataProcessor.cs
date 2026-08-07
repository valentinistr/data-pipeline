using Consumer.Models;

namespace Consumer.DataProcessors;

public interface IDataProcessor<TData, TRow>
{
    IngestionPackage<TData, TRow> Ingest(string filePath);
}