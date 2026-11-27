using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Application.Interfaces.IRepositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameOrEmailAsync(string identifier, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken cancellationToken = default);
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
}
