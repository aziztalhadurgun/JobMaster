using JobMaster.Core.Entities;
using JobMaster.Core.Models;

namespace JobMaster.Application.Interfaces;

public interface IJobService
{
    Task<JobLog> CreateJobAsync(JobDefinition jobDefinition);
    Task<JobLog?> GetJobLogAsync(string jobId);
    Task<IEnumerable<JobLog>> GetAllJobLogsAsync();
    Task<string> GetJobStateAsync(string jobId);
    Task DeleteJobAsync(string jobId);
    Task UpdateJobStatusAsync(int jobLogId, string status, string? errorMessage = null);
    Task ExecuteJobAsync(JobDefinition jobDefinition, int jobLogId);
}