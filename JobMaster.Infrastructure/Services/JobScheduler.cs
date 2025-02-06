using System.Linq.Expressions;
using Hangfire;
using JobMaster.Application.Interfaces;

namespace JobMaster.Infrastructure.Services;

public class JobScheduler(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager) : IJobScheduler
{
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager = recurringJobManager;

    public void EnqueueJob(Expression<Func<Task>> methodCall)
    {
        _backgroundJobClient.Enqueue(methodCall);
    }

    public void ScheduleJob(Expression<Func<Task>> methodCall, TimeSpan delay)
    {
        _backgroundJobClient.Schedule(methodCall, delay);
    }

    public void AddRecurringJob(Expression<Func<Task>> methodCall, string cronExpression, string jobId)
    {
        _recurringJobManager.AddOrUpdate(jobId, methodCall, cronExpression);
    }

    public string GetJobState(string jobId)
    {
        using var connection = JobStorage.Current.GetConnection();
        var jobData = connection.GetJobData(jobId);
        return jobData?.State ?? "NotFound";
    }

    public void DeleteJob(string jobId)
    {
        _backgroundJobClient.Delete(jobId);
    }
}