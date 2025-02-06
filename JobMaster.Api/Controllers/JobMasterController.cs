using JobMaster.Application.Interfaces;
using JobMaster.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobMaster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobMasterControllerr(IJobService jobService) : ControllerBase
{
    private readonly IJobService _jobService = jobService;

    [HttpPost("create")]
    public async Task<IActionResult> CreateJob([FromBody] JobDefinition jobDefinition)
    {
        try
        {
            var jobLog = await _jobService.CreateJobAsync(jobDefinition);
            var message = jobDefinition.TriggerType switch
            {
                JobTriggerType.Enqueue => $"Job '{jobDefinition.JobName}' enqueued successfully with ID: {jobLog.JobId}",
                JobTriggerType.Schedule => $"Job '{jobDefinition.JobName}' scheduled to run after {jobDefinition.DelayInMinutes} minutes with ID: {jobLog.JobId}",
                JobTriggerType.Recurring => $"Recurring job '{jobDefinition.JobName}' created with cron expression: {jobDefinition.CronExpression} and ID: {jobLog.JobId}",
                _ => throw new ArgumentException("Invalid trigger type")
            };
            return Ok(message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetJobLogs()
    {
        var logs = await _jobService.GetAllJobLogsAsync();
        return Ok(logs);
    }

    [HttpGet("logs/{jobId}")]
    public async Task<IActionResult> GetJobLog(string jobId)
    {
        var log = await _jobService.GetJobLogAsync(jobId);
        if (log == null)
            return NotFound($"Job with ID '{jobId}' not found");

        return Ok(log);
    }

    [HttpGet("{jobId}/state")]
    public async Task<IActionResult> GetJobState(string jobId)
    {
        try
        {
            var state = await _jobService.GetJobStateAsync(jobId);
            return Ok(new { JobId = jobId, State = state });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{jobId}")]
    public async Task<IActionResult> DeleteJob(string jobId)
    {
        try
        {
            await _jobService.DeleteJobAsync(jobId);
            return Ok($"Job '{jobId}' deleted successfully");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


}
