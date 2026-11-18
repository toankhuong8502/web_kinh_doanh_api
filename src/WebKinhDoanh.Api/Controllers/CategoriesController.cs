using Microsoft.AspNetCore.Mvc;
using WebKinhDoanh.Application.Interfaces.IServices;

namespace WebKinhDoanh.Api.Controllers;

[Route("api/categories")]
public sealed class CategoriesController(IProductService products) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken) =>
        Success(await products.GetCategoriesAsync(cancellationToken));
}
