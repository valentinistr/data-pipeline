using System.Globalization;
using CsvHelper;
using WorkerProcess.Models;

namespace WorkerProcess.DataProcessors;

public abstract class BaseDataProcessor<TData, TRow> : IDataProcessor<TData, TRow>
{
    public IngestionPackage<TData, TRow> Ingest(string filePath)
    {
        EnsureFileExists(filePath);

        var validRows = new List<TData>();
        var invalidRows = new List<TRow>();

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        foreach (var row in csv.GetRecords<TRow>())
        {
            if (ValidateRow(row))
            {
                validRows.Add(MapRow(row));
            }
            else
            {
                invalidRows.Add(row);
            }
        }
        
        return new IngestionPackage<TData, TRow>(validRows, invalidRows);
    }

    protected abstract bool ValidateRow(TRow row);
    
    protected abstract TData MapRow(TRow row);
    
    private static void EnsureFileExists(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Upload file was not found at '{filePath}'.", filePath);
        }
    }
}