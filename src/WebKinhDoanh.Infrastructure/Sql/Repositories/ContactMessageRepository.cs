using WebKinhDoanh.Application.Interfaces.IRepositories;
using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Infrastructure.Sql.Repositories;

public sealed class ContactMessageRepository(StoreDbContext dbContext) : IContactMessageRepository
{
    public async Task AddAsync(ContactMessage message, CancellationToken cancellationToken)
    {
        dbContext.ContactMessages.Add(message);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
