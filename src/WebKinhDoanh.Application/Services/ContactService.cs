using WebKinhDoanh.Application.Dtos.Contacts;
using WebKinhDoanh.Application.Interfaces.IRepositories;
using WebKinhDoanh.Application.Interfaces.IServices;
using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Application.Services;

public sealed class ContactService(IContactMessageRepository messages) : IContactService
{
    public Task CreateAsync(
        CreateContactMessageRequest request,
        CancellationToken cancellationToken) =>
        messages.AddAsync(
            new ContactMessage
            {
                Name = request.Name.Trim(),
                Email = request.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                Subject = request.Subject.Trim(),
                Message = request.Message.Trim(),
                CreatedAtUtc = DateTime.UtcNow
            },
            cancellationToken);
}
