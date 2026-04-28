using LogService.Application.Logs;
using LogService.Domain;
using SharedKernel;
using System.Threading.Tasks;
using System.Threading;

namespace LogService.Application.Abstractions;

public interface ILogApplicationService
{
    Task<Result<LogEntryDto>> CreateAsync(CreateLogRequest request, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<LogEntryDto>>> GetLatestAsync(int count, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<LogEntryDto>>> GetByLevelAsync(LogLevel level, int count, CancellationToken cancellationToken);
}