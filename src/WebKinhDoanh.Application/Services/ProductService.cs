using WebKinhDoanh.Application.Common;
using WebKinhDoanh.Application.Dtos.Common;
using WebKinhDoanh.Application.Dtos.Products;
using WebKinhDoanh.Application.Interfaces.IRepositories;
using WebKinhDoanh.Application.Interfaces.IServices;
using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Application.Services;

public sealed class ProductService(IProductRepository products) : IProductService
{
    public async Task<PagedResult<ProductDto>> GetProductsAsync(
        string? search,
        string? category,
        bool? featured,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (page < 1 || pageSize is < 1 or > 100)
        {
            throw new AppException(400, "Số trang phải lớn hơn 0 và pageSize từ 1 đến 100.");
        }

        var (items, totalCount) = await products.GetPageAsync(
            search?.Trim(),
            category?.Trim(),
            featured,
            (page - 1) * pageSize,
            pageSize,
            cancellationToken);

        return new PagedResult<ProductDto>(
            items.Select(Map).ToArray(),
            totalCount,
            page,
            pageSize);
    }

    public async Task<ProductDto> GetProductAsync(int id, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(id, cancellationToken);
        return product is null
            ? throw new AppException(404, "Không tìm thấy sản phẩm.")
            : Map(product);
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = await products.GetCategoriesAsync(cancellationToken);
        return categories
            .Select(category => new CategoryDto(
                category.Id,
                category.Name,
                category.Image,
                category.Products.Count(product => product.IsActive)))
            .ToArray();
    }

    private static ProductDto Map(Product product) =>
        new(
            product.Id,
            product.Name,
            product.Slug,
            product.CategoryId,
            product.Category.Name,
            product.Price,
            product.OldPrice,
            product.Rating,
            product.Image,
            product.Badge,
            product.Description,
            product.Detail,
            product.Stock);
}
