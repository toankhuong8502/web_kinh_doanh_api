namespace WebKinhDoanh.Application.Dtos.Products;

public sealed record ProductDto(
    int Id,
    string Name,
    string Slug,
    int CategoryId,
    string Category,
    decimal Price,
    decimal? OldPrice,
    int Rating,
    string Image,
    string? Badge,
    string Description,
    string Detail,
    int Stock);
