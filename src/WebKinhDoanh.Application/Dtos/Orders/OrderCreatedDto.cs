namespace WebKinhDoanh.Application.Dtos.Orders;

public sealed record OrderCreatedDto(
    string OrderNumber,
    decimal Subtotal,
    DateTime CreatedAtUtc);
