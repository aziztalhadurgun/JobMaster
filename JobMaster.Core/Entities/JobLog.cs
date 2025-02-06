using JobMaster.Core.Models;

namespace JobMaster.Core.Entities;

public class JobLog
{
    public int Id { get; set; }
    public string JobName { get; set; } = null!;
    public string JobId { get; set; } = null!;
    public string TargetUrl { get; set; } = null!;
    public string HttpMethod { get; set; } = null!;
    public string? Payload { get; set; }
    public JobTriggerType TriggerType { get; set; }
    public string? CronExpression { get; set; }
    public int? DelayInMinutes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? LastExecutedAt { get; set; }
    public string Status { get; set; } = null!;
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
}