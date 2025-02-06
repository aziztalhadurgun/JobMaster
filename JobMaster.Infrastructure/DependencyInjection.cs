using Hangfire;
using JobMaster.Application.Interfaces;
using JobMaster.Application.Services;
using JobMaster.Infrastructure.Data;
using JobMaster.Infrastructure.Repositories;
using JobMaster.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobMaster.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Add Hangfire
        services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"));
        });
        services.AddHangfireServer();

        // Add Hangfire Services
        services.AddScoped<IBackgroundJobClient, BackgroundJobClient>();
        services.AddScoped<IRecurringJobManager, RecurringJobManager>();

        // Add Services
        services.AddTransient<IJobScheduler, JobScheduler>();
        services.AddScoped<IJobLogRepository, JobLogRepository>();
        services.AddScoped<IJobService, JobService>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        // Enable Hangfire Dashboard
        app.UseHangfireDashboard("/hangfire");

        return app;
    }
}
