using Microsoft.AspNetCore.Mvc;
using WebKinhDoanh.Application.Dtos.Products;
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
        [FromQuery] int pageSize = 50,
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

    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await products.CreateProductAsync(request, cancellationToken);
        return Success(result, "Thêm sản phẩm thành công.");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(
        int id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await products.UpdateProductAsync(id, request, cancellationToken);
        return Success(result, "Cập nhật sản phẩm thành công.");
    }

    [HttpPatch("{id:int}/stock")]
    public async Task<IActionResult> UpdateStock(
        int id,
        [FromBody] UpdateStockRequest request,
        CancellationToken cancellationToken)
    {
        var result = await products.UpdateStockAsync(id, request, cancellationToken);
        return Success(result, "Cập nhật tồn kho thành công.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(
        int id,
        CancellationToken cancellationToken)
    {
        await products.DeleteProductAsync(id, cancellationToken);
        return SuccessMsg("Xóa sản phẩm thành công.");
    }
}
