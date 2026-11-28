namespace WebKinhDoanh.Application.Dtos.Products;

public sealed class UpdateStockRequest
{
    public int? Stock { get; set; }
    public int? Delta { get; set; }
}
