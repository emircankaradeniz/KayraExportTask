using LogService.Domain;
using System.Threading.Tasks;
using System.Threading;

namespace LogService.Application.Abstractions;

public interface ILogRepository
{
    Task AddAsync(LogEntry logEntry, CancellationToken cancellationToken);

    Task<IReadOnlyList<LogEntry>> GetLatestAsync(int count, CancellationToken cancellationToken);

    Task<IReadOnlyList<LogEntry>> GetByLevelAsync(LogLevel level, int count, CancellationToken cancellationToken);
}