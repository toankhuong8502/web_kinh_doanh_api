using WebKinhDoanh.Application.Dtos.Common;
using WebKinhDoanh.Domain.Entities;
using WebKinhDoanh.Domain.Enums;

namespace WebKinhDoanh.Application.Interfaces.IRepositories;

public interface IOrderRepository
{
    Task CreateAsync(
        Order order,
        IReadOnlyDictionary<int, int> quantitiesByProductId,
        CancellationToken cancellationToken);

    Task<PagedResult<Order>> GetOrdersAsync(
        string? search,
        OrderStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<Order?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken);
    Task<bool> UpdateStatusAsync(long id, OrderStatus status, CancellationToken cancellationToken);
}
