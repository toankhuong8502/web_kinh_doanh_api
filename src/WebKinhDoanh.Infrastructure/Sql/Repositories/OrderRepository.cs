using Microsoft.EntityFrameworkCore;
using WebKinhDoanh.Application.Common;
using WebKinhDoanh.Application.Dtos.Common;
using WebKinhDoanh.Application.Interfaces.IRepositories;
using WebKinhDoanh.Domain.Entities;
using WebKinhDoanh.Domain.Enums;

namespace WebKinhDoanh.Infrastructure.Sql.Repositories;

public sealed class OrderRepository(StoreDbContext dbContext) : IOrderRepository
{
    public async Task CreateAsync(
        Order order,
        IReadOnlyDictionary<int, int> quantitiesByProductId,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        foreach (var (productId, quantity) in quantitiesByProductId)
        {
            var affected = await dbContext.Products
                .Where(product => product.Id == productId && product.IsActive && product.Stock >= quantity)
                .ExecuteUpdateAsync(
                    updates => updates.SetProperty(
                        product => product.Stock,
                        product => product.Stock - quantity),
                    cancellationToken);

            if (affected != 1)
            {
                throw new AppException(409, "Sản phẩm vừa hết hàng hoặc không đủ số lượng tồn kho.");
            }
        }

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<PagedResult<Order>> GetOrdersAsync(
        string? search,
        OrderStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Orders
            .Include(o => o.Items)
            .AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var trimmed = search.Trim();
            query = query.Where(o =>
                o.OrderNumber.Contains(trimmed) ||
                o.CustomerName.Contains(trimmed) ||
                o.Phone.Contains(trimmed));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(o => o.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Order>(items, total, page, pageSize);
    }

    public async Task<Order?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<bool> UpdateStatusAsync(long id, OrderStatus status, CancellationToken cancellationToken)
    {
        var affected = await dbContext.Orders
            .Where(o => o.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.Status, status), cancellationToken);

        return affected > 0;
    }
}
