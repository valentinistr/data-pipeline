using Core.Models;
using WorkerProcess.Models;

namespace WorkerProcess.DataProcessors;

public class JobDataProcessor : BaseDataProcessor<Job, JobCsvRow>
{
    protected override bool ValidateRow(JobCsvRow row) => !string.IsNullOrWhiteSpace(row.JobCode);

    protected override Job MapRow(JobCsvRow row) => new()
    {
        JobCode = row.JobCode.Trim(),
        Name = row.Name?.Trim() ?? string.Empty,
    };
}