using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Application.Interfaces.IRepositories;

public interface IOrderRepository
{
    Task CreateAsync(
        Order order,
        IReadOnlyDictionary<int, int> quantitiesByProductId,
        CancellationToken cancellationToken);
}
