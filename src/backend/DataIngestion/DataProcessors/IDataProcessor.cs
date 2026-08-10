using DataIngestion.Models;

namespace DataIngestion.DataProcessors;

public interface IDataProcessor<TData, TRow>
{
    IngestionPackage<TData, TRow> Ingest(string filePath);
}
