using System.Threading.Tasks;
using System.Threading;

namespace ProductService.Application.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}