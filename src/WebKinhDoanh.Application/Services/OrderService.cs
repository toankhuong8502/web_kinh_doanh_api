using WebKinhDoanh.Application.Common;
using WebKinhDoanh.Application.Dtos.Orders;
using WebKinhDoanh.Application.Interfaces.IRepositories;
using WebKinhDoanh.Application.Interfaces.IServices;
using WebKinhDoanh.Domain.Entities;

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
}
