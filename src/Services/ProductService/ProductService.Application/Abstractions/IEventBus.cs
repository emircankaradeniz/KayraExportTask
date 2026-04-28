using System.Threading.Tasks;
using System.Threading;

namespace ProductService.Application.Abstractions;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent eventMessage, CancellationToken cancellationToken)
        where TEvent : class;
}