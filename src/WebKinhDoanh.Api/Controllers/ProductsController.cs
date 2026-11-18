using Microsoft.AspNetCore.Mvc;
using WebKinhDoanh.Application.Interfaces.IServices;

namespace WebKinhDoanh.Api.Controllers;

[Route("api/products")]
public sealed class ProductsController(IProductService products) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] bool? featured,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await products.GetProductsAsync(
            search,
            category,
            featured,
            page,
            pageSize,
            cancellationToken);
        return Success(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id, CancellationToken cancellationToken) =>
        Success(await products.GetProductAsync(id, cancellationToken));
}
