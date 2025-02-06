using System.Linq.Expressions;

namespace JobMaster.Application.Interfaces;

public interface IJobScheduler
{
    void EnqueueJob(Expression<Func<Task>> methodCall);
    void ScheduleJob(Expression<Func<Task>> methodCall, TimeSpan delay);
    void AddRecurringJob(Expression<Func<Task>> methodCall, string cronExpression, string jobId);
    string GetJobState(string jobId);
    void DeleteJob(string jobId);
}