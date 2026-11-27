using WebKinhDoanh.Application.Dtos.Auth;

namespace WebKinhDoanh.Application.Interfaces.IServices;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse> RegisterAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse?> GetCurrentUserAsync(string username, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoginResponse>> GetUsersAsync(CancellationToken cancellationToken = default);
}
