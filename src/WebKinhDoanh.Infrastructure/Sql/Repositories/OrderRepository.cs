using Microsoft.EntityFrameworkCore;
using WebKinhDoanh.Application.Common;
using WebKinhDoanh.Application.Interfaces.IRepositories;
using WebKinhDoanh.Domain.Entities;

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
}
