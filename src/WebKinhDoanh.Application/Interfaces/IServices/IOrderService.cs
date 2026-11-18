using WebKinhDoanh.Application.Dtos.Orders;

namespace WebKinhDoanh.Application.Interfaces.IServices;

public interface IOrderService
{
    Task<OrderCreatedDto> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken);
}
