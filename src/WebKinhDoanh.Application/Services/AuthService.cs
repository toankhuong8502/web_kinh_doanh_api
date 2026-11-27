using System.Security.Cryptography;
using System.Text;
using WebKinhDoanh.Application.Common;
using WebKinhDoanh.Application.Dtos.Auth;
using WebKinhDoanh.Application.Interfaces.IRepositories;
using WebKinhDoanh.Application.Interfaces.IServices;
using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Application.Services;

public sealed class AuthService(IUserRepository users) : IAuthService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new AppException(400, "Tên đăng nhập và mật khẩu không được để trống.");
        }

        var user = await users.GetByUsernameOrEmailAsync(request.Username.Trim(), cancellationToken);
        if (user is null || !user.IsActive)
        {
            throw new AppException(401, "Tài khoản hoặc mật khẩu không chính xác.");
        }

        if (!PasswordHasher.Verify(request.Password.Trim(), user.PasswordHash))
        {
            throw new AppException(401, "Tài khoản hoặc mật khẩu không chính xác.");
        }

        user.LastLoginAtUtc = DateTime.UtcNow;
        await users.UpdateAsync(user, cancellationToken);

        return ToResponse(user);
    }

    public async Task<LoginResponse> RegisterAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new AppException(400, "Vui lòng cung cấp đầy đủ thông tin tài khoản.");
        }

        if (await users.ExistsByUsernameOrEmailAsync(username, email, cancellationToken))
        {
            throw new AppException(400, "Tên đăng nhập hoặc email đã tồn tại trong hệ thống.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            FullName = string.IsNullOrWhiteSpace(request.FullName) ? username : request.FullName.Trim(),
            PasswordHash = PasswordHasher.Hash(request.Password.Trim()),
            Role = string.IsNullOrWhiteSpace(request.Role) ? "Admin" : request.Role.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var created = await users.AddAsync(user, cancellationToken);
        return ToResponse(created);
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new AppException(400, "Tên tài khoản không được để trống.");
        }

        if (string.IsNullOrWhiteSpace(request.OldPassword))
        {
            throw new AppException(400, "Vui lòng nhập mật khẩu cũ.");
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw new AppException(400, "Vui lòng nhập mật khẩu mới.");
        }

        if (request.NewPassword.Trim().Length < 6)
        {
            throw new AppException(400, "Mật khẩu mới phải có ít nhất 6 ký tự.");
        }

        if (request.NewPassword.Trim() != request.ConfirmPassword.Trim())
        {
            throw new AppException(400, "Mật khẩu mới và xác nhận mật khẩu không khớp.");
        }

        var user = await users.GetByUsernameOrEmailAsync(request.Username.Trim(), cancellationToken);
        if (user is null)
        {
            throw new AppException(404, "Không tìm thấy người dùng.");
        }

        if (!PasswordHasher.Verify(request.OldPassword.Trim(), user.PasswordHash))
        {
            throw new AppException(400, "Mật khẩu cũ không chính xác.");
        }

        user.PasswordHash = PasswordHasher.Hash(request.NewPassword.Trim());
        await users.UpdateAsync(user, cancellationToken);
    }

    public async Task<LoginResponse?> GetCurrentUserAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await users.GetByUsernameOrEmailAsync(username, cancellationToken);
        return user is null ? null : ToResponse(user);
    }

    public async Task<IReadOnlyList<LoginResponse>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var list = await users.GetAllAsync(cancellationToken);
        return list.Select(ToResponse).ToList();
    }

    private static LoginResponse ToResponse(User user)
    {
        var tokenPayload = $"{user.Id}:{user.Username}:{user.Role}:{DateTime.UtcNow.Ticks}";
        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenPayload));

        return new LoginResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            Token = token
        };
    }
}
