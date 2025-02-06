using System.Linq.Expressions;
using JobMaster.Application.Interfaces;
using JobMaster.Core.Entities;
using JobMaster.Core.Jobs;
using JobMaster.Core.Models;

namespace JobMaster.Application.Services;

public class JobService(IJobScheduler jobScheduler, IJobLogRepository jobLogRepository) : IJobService
{
    private readonly IJobScheduler _jobScheduler = jobScheduler;
    private readonly IJobLogRepository _jobLogRepository = jobLogRepository;


    public async Task ExecuteJobAsync(JobDefinition jobDefinition, int jobLogId)
    {
        try
        {
            using var job = new HttpJob(jobDefinition);
            await job.ExecuteAsync();
            await UpdateJobStatusAsync(jobLogId, "Completed");
        }
        catch (Exception ex)
        {
            await UpdateJobStatusAsync(jobLogId, "Failed", ex.Message);
            throw;
        }
    }

    public async Task<JobLog> CreateJobAsync(JobDefinition jobDefinition)
    {
        var jobLog = new JobLog
        {
            JobName = jobDefinition.JobName,
            JobId = Guid.NewGuid().ToString(),
            TargetUrl = jobDefinition.TargetUrl,
            HttpMethod = jobDefinition.HttpMethod,
            Payload = jobDefinition.Payload,
            TriggerType = jobDefinition.TriggerType,
            CronExpression = jobDefinition.CronExpression,
            DelayInMinutes = jobDefinition.DelayInMinutes,
            Status = "Created"
        };

        await _jobLogRepository.AddAsync(jobLog);

        var jobId = jobLog.Id;
        var jobDef = jobDefinition;

        Expression<Func<Task>> executeJob = () => ExecuteJobAsync(jobDef, jobId);

        switch (jobDefinition.TriggerType)
        {
            case JobTriggerType.Enqueue:
                _jobScheduler.EnqueueJob(executeJob);
                break;

            case JobTriggerType.Schedule:
                if (!jobDefinition.DelayInMinutes.HasValue)
                    throw new ArgumentException("DelayInMinutes is required for scheduled jobs");

                _jobScheduler.ScheduleJob(executeJob, TimeSpan.FromMinutes(jobDefinition.DelayInMinutes.Value));
                break;

            case JobTriggerType.Recurring:
                if (string.IsNullOrEmpty(jobDefinition.CronExpression))
                    throw new ArgumentException("CronExpression is required for recurring jobs");

                _jobScheduler.AddRecurringJob(executeJob, jobDefinition.CronExpression, jobLog.JobId);
                break;

            default:
                throw new ArgumentException("Invalid trigger type");
        }

        return jobLog;
    }

    public async Task<JobLog?> GetJobLogAsync(string jobId)
    {
        return await _jobLogRepository.GetByJobIdAsync(jobId);
    }

    public async Task<IEnumerable<JobLog>> GetAllJobLogsAsync()
    {
        return await _jobLogRepository.GetAllAsync();
    }

    public Task<string> GetJobStateAsync(string jobId)
    {
        if (string.IsNullOrEmpty(jobId))
            throw new ArgumentException("Job Id is required");

        return Task.FromResult(_jobScheduler.GetJobState(jobId));
    }

    public async Task DeleteJobAsync(string jobId)
    {
        if (string.IsNullOrEmpty(jobId))
            throw new ArgumentException("Job Id is required");

        var jobLog = await _jobLogRepository.GetByJobIdAsync(jobId);
        if (jobLog != null)
        {
            jobLog.Status = "Deleted";
            await _jobLogRepository.UpdateAsync(jobLog);
        }

        _jobScheduler.DeleteJob(jobId);
    }

    public async Task UpdateJobStatusAsync(int jobLogId, string status, string? errorMessage = null)
    {
        var jobLog = await _jobLogRepository.GetByIdAsync(jobLogId);
        if (jobLog != null)
        {
            jobLog.Status = status;
            jobLog.LastExecutedAt = DateTime.UtcNow;
            jobLog.ErrorMessage = errorMessage;
            if (errorMessage != null)
                jobLog.RetryCount++;

            await _jobLogRepository.UpdateAsync(jobLog);
        }
    }
}
