namespace WebKinhDoanh.Domain.Entities;

public sealed class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Description { get; set; }
    public required string Detail { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public required string Image { get; set; }
    public string? Badge { get; set; }
    public int Rating { get; set; }
    public int Stock { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
