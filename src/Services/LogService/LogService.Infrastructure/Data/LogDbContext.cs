using LogService.Application.Abstractions;
using LogService.Domain;
using Microsoft.EntityFrameworkCore;

namespace LogService.Infrastructure.Data;

public sealed class LogDbContext : DbContext, ILogUnitOfWork
{
    public LogDbContext(DbContextOptions<LogDbContext> options)
        : base(options)
    {
    }

    public DbSet<LogEntry> Logs => Set<LogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogDbContext).Assembly);
    }
}