namespace WebKinhDoanh.Domain.Entities;

public sealed class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Image { get; set; }
    public int DisplayOrder { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
