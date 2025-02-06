using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JobMaster.Core.Models;

public class JobDefinition
{
    [Required]
    public string JobName { get; set; } = null!;

    [Required]
    public string TargetUrl { get; set; } = null!;

    [Required]
    public string HttpMethod { get; set; } = "POST";

    public Dictionary<string, string> Headers { get; set; } = new();

    public string? Payload { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public JobTriggerType TriggerType { get; set; }

    public string? CronExpression { get; set; }

    public int? DelayInMinutes { get; set; }
}

public enum JobTriggerType
{
    Enqueue,
    Schedule,
    Recurring
}
