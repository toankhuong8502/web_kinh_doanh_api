using Microsoft.AspNetCore.Mvc;
using WebKinhDoanh.Application.Common;
using WebKinhDoanh.Application.Dtos.Orders;
using WebKinhDoanh.Application.Interfaces.IServices;

namespace WebKinhDoanh.Api.Controllers;

[Route("api/orders")]
public sealed class OrdersController(IOrderService orders) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var created = await orders.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<OrderCreatedDto>.Ok(created, "Đặt hàng thành công."));
    }
}
