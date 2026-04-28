using LogService.Application.Abstractions;
using LogService.Domain;
using LogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;

namespace LogService.Infrastructure.Repositories;

public sealed class LogRepository : ILogRepository
{
    private readonly LogDbContext _dbContext;

    public LogRepository(LogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(LogEntry logEntry, CancellationToken cancellationToken)
    {
        await _dbContext.Logs.AddAsync(logEntry, cancellationToken);
    }

    public async Task<IReadOnlyList<LogEntry>> GetLatestAsync(int count, CancellationToken cancellationToken)
    {
        return await _dbContext.Logs
            .OrderByDescending(log => log.CreatedAtUtc)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LogEntry>> GetByLevelAsync(LogLevel level, int count, CancellationToken cancellationToken)
    {
        return await _dbContext.Logs
            .Where(log => log.Level == level)
            .OrderByDescending(log => log.CreatedAtUtc)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}