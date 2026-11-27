using Microsoft.AspNetCore.Mvc;
using WebKinhDoanh.Application.Dtos.Auth;
using WebKinhDoanh.Application.Interfaces.IServices;

namespace WebKinhDoanh.Api.Controllers;

[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        return Success(result, "Đăng nhập thành công.");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        return Success(result, "Tạo tài khoản thành công.");
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        await authService.ChangePasswordAsync(request, cancellationToken);
        return SuccessMsg("Đổi mật khẩu thành công.");
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken = default)
    {
        var result = await authService.GetUsersAsync(cancellationToken);
        return Success(result);
    }
}
