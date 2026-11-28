using WebKinhDoanh.Application.Dtos.Common;
using WebKinhDoanh.Application.Dtos.Orders;

namespace WebKinhDoanh.Application.Interfaces.IServices;

public interface IOrderService
{
    Task<OrderCreatedDto> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken);

    Task<PagedResult<OrderDto>> GetOrdersAsync(
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<OrderDto?> GetOrderByIdOrNumberAsync(
        string idOrNumber,
        CancellationToken cancellationToken);

    Task<bool> UpdateStatusAsync(
        string idOrNumber,
        string status,
        CancellationToken cancellationToken);
}
