using WebKinhDoanh.Application.Dtos.Common;
using WebKinhDoanh.Application.Dtos.Products;

namespace WebKinhDoanh.Application.Interfaces.IServices;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetProductsAsync(
        string? search,
        string? category,
        bool? featured,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<ProductDto> GetProductAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken);
}
