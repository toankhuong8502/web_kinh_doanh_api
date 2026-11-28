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
    Task<Product?> GetTrackingByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> GetByIdsAsync(
        IReadOnlyCollection<int> ids,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<Category?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken);
    Task<Category?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken);
    Task UpdateAsync(Product product, CancellationToken cancellationToken);
    Task DeleteAsync(Product product, CancellationToken cancellationToken);
}
