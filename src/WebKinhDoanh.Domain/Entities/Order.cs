using WebKinhDoanh.Domain.Enums;

namespace WebKinhDoanh.Domain.Entities;

public sealed class Order
{
    public long Id { get; set; }
    public required string OrderNumber { get; set; }
    public required string CustomerName { get; set; }
    public required string Phone { get; set; }
    public string? Email { get; set; }
    public required string Address { get; set; }
    public string? Note { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal Subtotal { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
