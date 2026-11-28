using WebKinhDoanh.Application.Common;
using WebKinhDoanh.Application.Dtos.Common;
using WebKinhDoanh.Application.Dtos.Orders;
using WebKinhDoanh.Application.Interfaces.IRepositories;
using WebKinhDoanh.Application.Interfaces.IServices;
using WebKinhDoanh.Domain.Entities;
using WebKinhDoanh.Domain.Enums;

namespace WebKinhDoanh.Application.Services;

public sealed class OrderService(
    IProductRepository products,
    IOrderRepository orders) : IOrderService
{
    private static readonly HashSet<string> ClothingSizes = ["S", "M", "L", "XL"];

    public async Task<OrderCreatedDto> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var productIds = request.Items.Select(item => item.ProductId).Distinct().ToArray();
        var productList = await products.GetByIdsAsync(productIds, cancellationToken);
        var productById = productList.ToDictionary(product => product.Id);

        if (productById.Count != productIds.Length)
        {
            throw new AppException(400, "Giỏ hàng có sản phẩm không tồn tại hoặc đã ngừng kinh doanh.");
        }

        var quantities = request.Items
            .GroupBy(item => item.ProductId)
            .ToDictionary(group => group.Key, group => group.Sum(item => item.Quantity));

        foreach (var item in request.Items)
        {
            var product = productById[item.ProductId];
            var clothing = product.Category.Name.Equals("Quần áo nữ", StringComparison.OrdinalIgnoreCase);
            if (clothing && !ClothingSizes.Contains(item.Size ?? string.Empty))
            {
                throw new AppException(400, $"Vui lòng chọn kích cỡ hợp lệ cho sản phẩm \"{product.Name}\".");
            }

            if (!clothing && !string.IsNullOrWhiteSpace(item.Size))
            {
                throw new AppException(400, $"Sản phẩm \"{product.Name}\" không có lựa chọn kích cỡ.");
            }
        }

        foreach (var (productId, quantity) in quantities)
        {
            if (productById[productId].Stock < quantity)
            {
                throw new AppException(409, $"Sản phẩm \"{productById[productId].Name}\" không đủ tồn kho.");
            }
        }

        var createdAt = DateTime.UtcNow;
        var order = new Order
        {
            OrderNumber = $"MM{createdAt:yyyyMMdd}{Guid.NewGuid():N}"[..18].ToUpperInvariant(),
            CustomerName = request.CustomerName.Trim(),
            Phone = request.Phone.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Address = request.Address.Trim(),
            Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
            CreatedAtUtc = createdAt,
            Items = request.Items.Select(item =>
            {
                var product = productById[item.ProductId];
                return new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = item.Quantity,
                    Size = string.IsNullOrWhiteSpace(item.Size) ? null : item.Size.Trim().ToUpperInvariant()
                };
            }).ToList()
        };
        order.Subtotal = order.Items.Sum(item => item.UnitPrice * item.Quantity);

        await orders.CreateAsync(order, quantities, cancellationToken);
        return new OrderCreatedDto(order.OrderNumber, order.Subtotal, createdAt);
    }

    public async Task<PagedResult<OrderDto>> GetOrdersAsync(
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var parsedStatus = ParseStatus(status);
        var result = await orders.GetOrdersAsync(search, parsedStatus, page, pageSize, cancellationToken);
        var dtos = result.Items.Select(MapToDto).ToList();
        return new PagedResult<OrderDto>(dtos, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<OrderDto?> GetOrderByIdOrNumberAsync(string idOrNumber, CancellationToken cancellationToken)
    {
        Order? order = null;
        if (long.TryParse(idOrNumber, out var id))
        {
            order = await orders.GetByIdAsync(id, cancellationToken);
        }

        order ??= await orders.GetByOrderNumberAsync(idOrNumber, cancellationToken);
        return order is null ? null : MapToDto(order);
    }

    public async Task<bool> UpdateStatusAsync(string idOrNumber, string status, CancellationToken cancellationToken)
    {
        var targetStatus = ParseStatus(status);
        if (!targetStatus.HasValue)
        {
            throw new AppException(400, $"Trạng thái \"{status}\" không hợp lệ.");
        }

        long orderId = 0;
        if (long.TryParse(idOrNumber, out var id))
        {
            orderId = id;
        }
        else
        {
            var found = await orders.GetByOrderNumberAsync(idOrNumber, cancellationToken);
            if (found is null)
            {
                throw new AppException(404, "Không tìm thấy đơn hàng.");
            }
            orderId = found.Id;
        }

        return await orders.UpdateStatusAsync(orderId, targetStatus.Value, cancellationToken);
    }

    private static OrderStatus? ParseStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status) || status.Equals("Tất cả", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return status.Trim().ToLowerInvariant() switch
        {
            "chờ xác nhận" or "pending" => OrderStatus.Pending,
            "đã xác nhận" or "confirmed" => OrderStatus.Confirmed,
            "đang xử lý" or "processing" => OrderStatus.Processing,
            "đang giao" or "shipped" => OrderStatus.Shipped,
            "hoàn thành" or "completed" => OrderStatus.Completed,
            "đã hủy" or "cancelled" => OrderStatus.Cancelled,
            _ => null
        };
    }

    private static string MapStatusToVietnamese(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "Chờ xác nhận",
        OrderStatus.Confirmed => "Chờ xác nhận",
        OrderStatus.Processing => "Đang giao",
        OrderStatus.Shipped => "Đang giao",
        OrderStatus.Completed => "Hoàn thành",
        OrderStatus.Cancelled => "Đã hủy",
        _ => "Chờ xác nhận"
    };

    private static OrderDto MapToDto(Order o) => new(
        o.Id,
        o.OrderNumber,
        o.CustomerName,
        o.Phone,
        o.Email,
        o.Address,
        o.Note,
        MapStatusToVietnamese(o.Status),
        o.Status.ToString(),
        o.Subtotal,
        o.CreatedAtUtc,
        o.Items.Select(i => new OrderItemDto(
            i.Id,
            i.ProductId,
            i.ProductName,
            i.UnitPrice,
            i.Quantity,
            i.Size)).ToList());
}
