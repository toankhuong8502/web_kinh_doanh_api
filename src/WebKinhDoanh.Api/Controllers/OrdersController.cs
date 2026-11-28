using Microsoft.AspNetCore.Mvc;
using WebKinhDoanh.Application.Dtos.Orders;
using WebKinhDoanh.Application.Interfaces.IServices;

namespace WebKinhDoanh.Api.Controllers;

[Route("api/orders")]
public sealed class OrdersController(IOrderService orders) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await orders.GetOrdersAsync(search, status, page, pageSize, cancellationToken);
        return Success(result);
    }

    [HttpGet("{idOrNumber}")]
    public async Task<IActionResult> GetOrder(
        string idOrNumber,
        CancellationToken cancellationToken)
    {
        var order = await orders.GetOrderByIdOrNumberAsync(idOrNumber, cancellationToken);
        if (order is null)
        {
            return NotFound(new { message = "Không tìm thấy đơn hàng." });
        }
        return Success(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var created = await orders.CreateAsync(request, cancellationToken);
        return Success(created, "Đặt hàng thành công.");
    }

    [HttpPatch("{idOrNumber}/status")]
    public async Task<IActionResult> UpdateStatus(
        string idOrNumber,
        [FromBody] UpdateOrderStatusRequest request,
        CancellationToken cancellationToken)
    {
        await orders.UpdateStatusAsync(idOrNumber, request.Status, cancellationToken);
        return SuccessMsg("Cập nhật trạng thái đơn hàng thành công.");
    }
}
