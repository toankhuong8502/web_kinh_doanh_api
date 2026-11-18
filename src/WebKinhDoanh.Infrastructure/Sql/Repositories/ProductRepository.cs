using Microsoft.EntityFrameworkCore;
using WebKinhDoanh.Application.Interfaces.IRepositories;
using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Infrastructure.Sql.Repositories;

public sealed class ProductRepository(StoreDbContext dbContext) : IProductRepository
{
    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPageAsync(
        string? search,
        string? category,
        bool? featured,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .Where(product => product.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(product =>
                product.Name.Contains(search) ||
                product.Category.Name.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(product => product.Category.Name == category);
        }

        if (featured.HasValue)
        {
            query = query.Where(product => product.IsFeatured == featured.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(product => product.IsFeatured)
            .ThenBy(product => product.Id)
            .Skip(skip)
            .Take(take)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .FirstOrDefaultAsync(
                product => product.Id == id && product.IsActive,
                cancellationToken);

    public async Task<IReadOnlyList<Product>> GetByIdsAsync(
        IReadOnlyCollection<int> ids,
        CancellationToken cancellationToken) =>
        await dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .Where(product => ids.Contains(product.Id) && product.IsActive)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken) =>
        await dbContext.Categories
            .AsNoTracking()
            .Include(category => category.Products)
            .OrderBy(category => category.DisplayOrder)
            .ToArrayAsync(cancellationToken);
}
