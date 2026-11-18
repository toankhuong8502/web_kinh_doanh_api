namespace WebKinhDoanh.Application.Dtos.Contacts;

public sealed class CreateContactMessageRequest
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string? Phone { get; init; }
    public required string Subject { get; init; }
    public required string Message { get; init; }
}
