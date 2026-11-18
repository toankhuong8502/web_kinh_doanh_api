namespace WebKinhDoanh.Domain.Entities;

public sealed class OrderItem
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string? Size { get; set; }
}
