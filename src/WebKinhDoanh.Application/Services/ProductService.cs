using System.Text.RegularExpressions;
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
        if (page < 1 || pageSize is < 1 or > 500)
        {
            throw new AppException(400, "Số trang phải lớn hơn 0 và pageSize từ 1 đến 500.");
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

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new AppException(400, "Tên sản phẩm không được để trống.");
        }

        if (request.Price <= 0)
        {
            throw new AppException(400, "Giá bán sản phẩm phải lớn hơn 0.");
        }

        Category? category = null;
        if (request.CategoryId.HasValue)
        {
            category = await products.GetCategoryByIdAsync(request.CategoryId.Value, cancellationToken);
        }
        if (category is null && !string.IsNullOrWhiteSpace(request.Category))
        {
            category = await products.GetCategoryByNameAsync(request.Category, cancellationToken);
        }
        if (category is null)
        {
            var categories = await products.GetCategoriesAsync(cancellationToken);
            category = categories.FirstOrDefault() ?? throw new AppException(400, "Chưa có danh mục sản phẩm nào trong hệ thống.");
        }

        var slug = GenerateSlug(request.Name);
        var product = new Product
        {
            Name = request.Name.Trim(),
            Slug = slug,
            CategoryId = category.Id,
            Category = category,
            Price = request.Price,
            OldPrice = request.OldPrice,
            Stock = Math.Max(0, request.Stock),
            Image = string.IsNullOrWhiteSpace(request.Image) ? "photo-1596462502278-27bfdc403348" : request.Image.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? request.Name.Trim() : request.Description.Trim(),
            Detail = string.IsNullOrWhiteSpace(request.Detail) ? $"{category.Name} · Xuất xứ: Việt Nam" : request.Detail.Trim(),
            Badge = request.Badge,
            Rating = 5,
            IsFeatured = request.IsFeatured,
            IsActive = true
        };

        var created = await products.AddAsync(product, cancellationToken);
        return Map(created);
    }

    public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await products.GetTrackingByIdAsync(id, cancellationToken);
        if (product is null)
        {
            throw new AppException(404, "Không tìm thấy sản phẩm.");
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            product.Name = request.Name.Trim();
            product.Slug = GenerateSlug(request.Name);
        }

        if (request.Price.HasValue)
        {
            if (request.Price.Value <= 0)
            {
                throw new AppException(400, "Giá bán phải lớn hơn 0.");
            }
            product.Price = request.Price.Value;
        }

        if (request.OldPrice.HasValue)
        {
            product.OldPrice = request.OldPrice.Value;
        }

        if (request.Stock.HasValue)
        {
            product.Stock = Math.Max(0, request.Stock.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Image))
        {
            product.Image = request.Image.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            product.Description = request.Description.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Detail))
        {
            product.Detail = request.Detail.Trim();
        }

        if (request.Badge != null)
        {
            product.Badge = string.IsNullOrWhiteSpace(request.Badge) ? null : request.Badge.Trim();
        }

        if (request.IsFeatured.HasValue)
        {
            product.IsFeatured = request.IsFeatured.Value;
        }

        if (request.CategoryId.HasValue)
        {
            var cat = await products.GetCategoryByIdAsync(request.CategoryId.Value, cancellationToken);
            if (cat != null)
            {
                product.CategoryId = cat.Id;
                product.Category = cat;
            }
        }
        else if (!string.IsNullOrWhiteSpace(request.Category))
        {
            var cat = await products.GetCategoryByNameAsync(request.Category, cancellationToken);
            if (cat != null)
            {
                product.CategoryId = cat.Id;
                product.Category = cat;
            }
        }

        await products.UpdateAsync(product, cancellationToken);
        return Map(product);
    }

    public async Task<ProductDto> UpdateStockAsync(int id, UpdateStockRequest request, CancellationToken cancellationToken)
    {
        var product = await products.GetTrackingByIdAsync(id, cancellationToken);
        if (product is null)
        {
            throw new AppException(404, "Không tìm thấy sản phẩm.");
        }

        if (request.Stock.HasValue)
        {
            product.Stock = Math.Max(0, request.Stock.Value);
        }
        else if (request.Delta.HasValue)
        {
            product.Stock = Math.Max(0, product.Stock + request.Delta.Value);
        }

        await products.UpdateAsync(product, cancellationToken);
        return Map(product);
    }

    public async Task DeleteProductAsync(int id, CancellationToken cancellationToken)
    {
        var product = await products.GetTrackingByIdAsync(id, cancellationToken);
        if (product is null)
        {
            throw new AppException(404, "Không tìm thấy sản phẩm.");
        }

        await products.DeleteAsync(product, cancellationToken);
    }

    private static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return $"sp-{DateTime.UtcNow.Ticks}";

        var normalized = text.ToLowerInvariant().Trim();
        string[] vietnameseSigns = [
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        ];
        for (int i = 1; i < vietnameseSigns.Length; i++)
        {
            for (int j = 0; j < vietnameseSigns[i].Length; j++)
                normalized = normalized.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
        }
        var slug = Regex.Replace(normalized, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? $"sp-{DateTime.UtcNow.Ticks}" : slug;
    }

    private static ProductDto Map(Product product) =>
        new(
            product.Id,
            product.Name,
            product.Slug,
            product.CategoryId,
            product.Category?.Name ?? "Chưa phân loại",
            product.Price,
            product.OldPrice,
            product.Rating,
            product.Image,
            product.Badge,
            product.Description,
            product.Detail,
            product.Stock);
}
