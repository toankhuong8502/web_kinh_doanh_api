namespace WebKinhDoanh.Application.Dtos.Auth;

public sealed class LoginResponse
{
    public Guid Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string FullName { get; init; }
    public required string Role { get; init; }
    public required string Token { get; init; }
}
