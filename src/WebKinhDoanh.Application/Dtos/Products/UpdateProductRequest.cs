namespace WebKinhDoanh.Application.Dtos.Products;

public sealed class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Category { get; set; }
    public int? CategoryId { get; set; }
    public decimal? Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int? Stock { get; set; }
    public string? Image { get; set; }
    public string? Description { get; set; }
    public string? Detail { get; set; }
    public string? Badge { get; set; }
    public bool? IsFeatured { get; set; }
}
