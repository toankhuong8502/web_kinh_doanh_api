namespace WebKinhDoanh.Domain.Entities;

public sealed class ContactMessage
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public required string Subject { get; set; }
    public required string Message { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
