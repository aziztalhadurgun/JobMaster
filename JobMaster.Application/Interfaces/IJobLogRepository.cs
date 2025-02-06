using JobMaster.Core.Entities;

namespace JobMaster.Application.Interfaces;

public interface IJobLogRepository
{
    Task<JobLog> AddAsync(JobLog jobLog);
    Task<JobLog?> GetByIdAsync(int id);
    Task<JobLog?> GetByJobIdAsync(string jobId);
    Task<IEnumerable<JobLog>> GetAllAsync();
    Task UpdateAsync(JobLog jobLog);
}
