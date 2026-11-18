using Microsoft.AspNetCore.Mvc;
using WebKinhDoanh.Application.Common;
using WebKinhDoanh.Application.Dtos.Contacts;
using WebKinhDoanh.Application.Interfaces.IServices;

namespace WebKinhDoanh.Api.Controllers;

[Route("api/contact")]
public sealed class ContactController(IContactService contacts) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateMessage(
        CreateContactMessageRequest request,
        CancellationToken cancellationToken)
    {
        await contacts.CreateAsync(request, cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<object?>.Ok(null, "Đã nhận thông tin liên hệ."));
    }
}
