using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Application.Interfaces.IRepositories;

public interface IProductRepository
{
    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPageAsync(
        string? search,
        string? category,
        bool? featured,
        int skip,
        int take,
        CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> GetByIdsAsync(
        IReadOnlyCollection<int> ids,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken);
}
