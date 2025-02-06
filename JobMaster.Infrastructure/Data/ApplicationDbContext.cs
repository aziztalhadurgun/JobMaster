using JobMaster.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobMaster.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<JobLog> JobLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.JobName).IsRequired();
            entity.Property(e => e.JobId).IsRequired();
            entity.Property(e => e.TargetUrl).IsRequired();
            entity.Property(e => e.HttpMethod).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedDate).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.RetryCount).HasDefaultValue(0);
        });
    }
}