namespace WebKinhDoanh.Application.Dtos.Orders;

public sealed record OrderDto(
    long Id,
    string OrderNumber,
    string CustomerName,
    string Phone,
    string? Email,
    string Address,
    string? Note,
    string Status,
    string StatusRaw,
    decimal Subtotal,
    DateTime CreatedAtUtc,
    IReadOnlyList<OrderItemDto> Items);

public sealed record OrderItemDto(
    long Id,
    int ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    string? Size);
