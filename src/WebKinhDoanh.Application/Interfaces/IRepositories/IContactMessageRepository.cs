using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Application.Interfaces.IRepositories;

public interface IContactMessageRepository
{
    Task AddAsync(ContactMessage message, CancellationToken cancellationToken);
}
