using WorkerProcess.Models;

namespace WorkerProcess.DataProcessors;

public interface IDataProcessor<TData, TRow>
{
    IngestionPackage<TData, TRow> Ingest(string filePath);
}