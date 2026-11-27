using Microsoft.EntityFrameworkCore;
using WebKinhDoanh.Application.Interfaces.IRepositories;
using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Infrastructure.Sql.Repositories;

public sealed class UserRepository(StoreDbContext db) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await db.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<User?> GetByUsernameOrEmailAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var normalized = identifier.Trim().ToLower();
        return await db.Users.FirstOrDefaultAsync(
            x => x.Username.ToLower() == normalized || x.Email.ToLower() == normalized,
            cancellationToken);
    }

    public async Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken cancellationToken = default)
    {
        var normUser = username.Trim().ToLower();
        var normEmail = email.Trim().ToLower();
        return await db.Users.AnyAsync(
            x => x.Username.ToLower() == normUser || x.Email.ToLower() == normEmail,
            cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await db.Users.AddAsync(user, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        db.Users.Update(user);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Users.AsNoTracking().OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
}
