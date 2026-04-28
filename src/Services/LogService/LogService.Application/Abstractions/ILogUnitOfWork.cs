using System.Threading.Tasks;
using System.Threading;

namespace LogService.Application.Abstractions;

public interface ILogUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}