using System.Text;
using System.Text.Json;
using JobMaster.Core.Models;

namespace JobMaster.Core.Jobs;

public class HttpJob(JobDefinition jobDefinition) : IDisposable
{
    private readonly HttpClient _httpClient = new();
    private readonly JobDefinition _jobDefinition = jobDefinition ?? throw new ArgumentNullException(nameof(jobDefinition));

    public async Task ExecuteAsync()
    {
        try
        {
            var request = new HttpRequestMessage(new HttpMethod(_jobDefinition.HttpMethod), _jobDefinition.TargetUrl);

            if (_jobDefinition.Headers?.Count > 0)
            {
                foreach (var header in _jobDefinition.Headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (_jobDefinition.Payload != null)
            {
                var json = JsonSerializer.Serialize(_jobDefinition.Payload);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing job {_jobDefinition.JobName}: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
