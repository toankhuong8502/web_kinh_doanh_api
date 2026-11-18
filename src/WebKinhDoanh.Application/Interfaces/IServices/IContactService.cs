using WebKinhDoanh.Application.Dtos.Contacts;

namespace WebKinhDoanh.Application.Interfaces.IServices;

public interface IContactService
{
    Task CreateAsync(CreateContactMessageRequest request, CancellationToken cancellationToken);
}
