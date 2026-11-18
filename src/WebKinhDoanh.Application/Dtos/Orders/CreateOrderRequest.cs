namespace WebKinhDoanh.Application.Dtos.Orders;

public sealed class CreateOrderRequest
{
    public required string CustomerName { get; init; }
    public required string Phone { get; init; }
    public string? Email { get; init; }
    public required string Address { get; init; }
    public string? Note { get; init; }
    public required List<CreateOrderItemRequest> Items { get; init; }
}

public sealed class CreateOrderItemRequest
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
    public string? Size { get; init; }
}
