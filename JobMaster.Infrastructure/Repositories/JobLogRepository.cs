using JobMaster.Application.Interfaces;
using JobMaster.Core.Entities;
using JobMaster.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobMaster.Infrastructure.Repositories;

public class JobLogRepository(ApplicationDbContext context) : IJobLogRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<JobLog> AddAsync(JobLog jobLog)
    {
        await _context.JobLogs.AddAsync(jobLog);
        await _context.SaveChangesAsync();
        return jobLog;
    }

    public async Task<JobLog?> GetByIdAsync(int id)
    {
        return await _context.JobLogs.FindAsync(id);
    }

    public async Task<JobLog?> GetByJobIdAsync(string jobId)
    {
        return await _context.JobLogs.FirstOrDefaultAsync(x => x.JobId == jobId);
    }

    public async Task<IEnumerable<JobLog>> GetAllAsync()
    {
        return await _context.JobLogs.OrderByDescending(x => x.CreatedDate).ToListAsync();
    }

    public async Task UpdateAsync(JobLog jobLog)
    {
        jobLog.UpdatedDate = DateTime.UtcNow;
        _context.JobLogs.Update(jobLog);
        await _context.SaveChangesAsync();
    }
}
